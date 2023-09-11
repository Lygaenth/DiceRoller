﻿using DiceRoller.Core.Apis;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace DiceRollerServer.Hubs
{
    public class RollHub : Hub
	{
		private readonly IRollService _rollService;
		private readonly IPartyService _partyService;
		private readonly IMapService _mapService;

		public RollHub(IRollService rollService, IPartyService partyService, IMapService mapService)
		{
			_rollService = rollService;
			_partyService = partyService;
			_mapService = mapService;
		}

		public async Task SendMessage(string partyIdStr, string userIdStr, string message)
		{
			if (!Int32.TryParse(partyIdStr, out int partyId))
				return;
			if (!Int32.TryParse(userIdStr, out int userId) && userIdStr.ToUpper() != "GM")
				return;
            await Clients.Group(partyIdStr).SendAsync("ReceiveMessage", _partyService.GetUserName(partyId, userId), message);
		}

		public async Task RollDie(string partyIdStr, string userIdStr, int die)
		{
            if (!Int32.TryParse(partyIdStr, out int partyId))
                return;
            if (!Int32.TryParse(userIdStr, out int userId) && userIdStr.ToUpper() != "GM")
                return;

            int result = _rollService.RollDie(userId, die);
            await Clients.Group(partyIdStr).SendAsync("ReceiveRollResult", _partyService.GetUserName(partyId, userId), result, die);
		}

		public async Task CreateSession(string name, string password)
		{
			try
			{
				var party = _partyService.CreateSession(name, password);
				_mapService.LoadPartyMaps(party.ID);
				await Groups.AddToGroupAsync(Context.ConnectionId, "Party_"+party.ID);
				await Clients.Caller.SendAsync("SessionCreated", party.ID);
				Context.User.AddIdentity(new System.Security.Claims.ClaimsIdentity("RollHubId", party.ID + "_GM", "GM"));
			}
			catch
			{
				await Clients.Caller.SendAsync("FailedToCreateSession");
			}
		}

		public async Task<string> GetPartyName(string partyIdStr)
		{
			if (Int32.TryParse(partyIdStr, out int partyId))
				return _partyService.GetParty(partyId).Name;
			return "";
		}

		public async Task<string> GetUser(string partyIdStr, string userIdStr)
		{
            if (!Int32.TryParse(partyIdStr, out int partyId))
                return "";
            if (!Int32.TryParse(userIdStr, out int userId) && userIdStr.ToUpper() != "GM")
                return "";

			return _partyService.GetUserName(partyId, userId);
		}

		private string GetUsers(string partyIdStr)
		{
            var res = "";

			if (Int32.TryParse(partyIdStr, out int partyId))
			{
				var users = _partyService.GetUsers(partyId);
				foreach (var user in users)
					res += user.ID + ";" + user.Name + ";" + user.HpMax + ";" + user.Hp+";"+user.ImageUrl+";"+user.Position.X+";"+user.Position.Y+"|";
				res = res.Substring(0, res.Length - 1);
			}

			return res;
		}

		public async Task ConnectPlayer(string name, string partyIdStr, string password)
		{
            if (Int32.TryParse(partyIdStr, out var partyId))
            {
                var userId = _partyService.Connect(partyId, name, password);
                if (userId >= 1)
                {
                    await Clients.Caller.SendAsync("CanJoinSessionAsPlayer", partyId, userId, name);
                    return;
                }
            }
            await Clients.Caller.SendAsync("FailedToJoinSession");

        }

        public async Task JoinPlayer(string partyIdStr, string userIdStr)
		{
			if (Int32.TryParse(partyIdStr, out var partyId))
			{
				int userId = 0;

				if (userIdStr.ToUpper() != "GM" && !Int32.TryParse(userIdStr, out userId))
					return;

                string name = _partyService.GetUserName(partyId, userId);
				await Groups.AddToGroupAsync(Context.ConnectionId, partyIdStr);
				if (userId >= 1)
				{
					await Clients.Caller.SendAsync("JoinSessionAsPlayer", partyId, userId, name);
                    await Clients.OthersInGroup(partyId.ToString()).SendAsync("JoinedUser", partyId, name);
                    await Clients.Groups(partyId.ToString()).SendAsync("UsersUpdated", GetUsers(partyIdStr));
                    return;
				}
            }
            await Clients.Caller.SendAsync("FailedToJoinSession");
        }

		public async Task MoveImage(string partyIdStr, string imageIdStr, string X, string Y)
		{
			if (!Int32.TryParse(partyIdStr, out var partyId))
				return;

            if (!Int32.TryParse(imageIdStr, out var userId))
                return;

            if (!Int32.TryParse(X, out var x))
                return;

            if (!Int32.TryParse(Y, out var y))
                return;

			if (_partyService.MoveUser(partyId, userId, new System.Drawing.Point(x, y)))
				await Clients.OthersInGroup(partyId.ToString()).SendAsync("ImageMoved", userId, X, Y);
			else
			{
				var position = _partyService.GetUserPosition(partyId, userId);
				await Clients.Caller.SendAsync("ImageMoved", userId, position.X, position.Y);
			}
		}

		public async Task LoadBackground(string partyIdStr, string mapIdStr)
		{
            if (!Int32.TryParse(partyIdStr, out var partyId))
                return;

            if (!Int32.TryParse(mapIdStr, out var mapId))
                return;

            var map = _mapService.GetMap(partyId, mapId);
			await Clients.Group(partyId.ToString()).SendAsync("UpdatedBackground", map.Url, map.TileNumber);
		}
	}
}
