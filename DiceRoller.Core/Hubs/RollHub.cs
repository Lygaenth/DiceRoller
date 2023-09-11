﻿using DiceRollerServer.Services;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace DiceRollerServer.Hubs
{
	public class RollHub : Hub
	{
		private readonly RollService _rollService;
		private readonly PartyService _partyService;
		private readonly MapService _mapService;

		public RollHub(RollService rollService, PartyService partyService, MapService mapService)
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
            await Clients.All.SendAsync("ReceiveMessage", _partyService.GetUser(partyId, userId), message);
            //await Clients.Group(partyIdStr).SendAsync("ReceiveMessage", _partyService.GetUser(partyId, userId), message);
		}

		public async Task RollDie(string partyIdStr, string userIdStr, int die)
		{
            if (!Int32.TryParse(partyIdStr, out int partyId))
                return;
            if (!Int32.TryParse(userIdStr, out int userId) && userIdStr.ToUpper() != "GM")
                return;

            int result = _rollService.RollDie(die);
            await Clients.All.SendAsync("ReceiveRollResult", _partyService.GetUser(partyId, userId), result, die);
            //await Clients.Group(partyIdStr).SendAsync("ReceiveRollResult", _partyService.GetUser(partyId, userId), result, die);
		}

		public async Task CreateSession(string name, string password)
		{
			try
			{
				var party = _partyService.CreateSession(name, password);
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

			return _partyService.GetUser(partyId, userId);
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

                string name = _partyService.GetUser(partyId, userId);
				await Groups.AddToGroupAsync(Context.ConnectionId, partyIdStr);
				if (userId >= 1)
				{
					await Clients.Caller.SendAsync("JoinSessionAsPlayer", partyId, userId, name);
					await Clients.Others.SendAsync("JoinedUser", partyId, name);
                    await Clients.All.SendAsync("UsersUpdated", GetUsers(partyIdStr));
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
				await Clients.Others.SendAsync("ImageMoved", userId, X, Y);
			else
			{
				var position = _partyService.GetUserPosition(partyId, userId);
				await Clients.Caller.SendAsync("ImageMoved", userId, position.X, position.Y);
			}
		}

		public async Task LoadBackground(string background)
		{
			var map = _mapService.GetMap(background);
			await Clients.All.SendAsync("UpdatedBackground", background, map.TileNumber);
		}
	}
}
