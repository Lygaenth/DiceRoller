using DiceRoller.Core.Apis;
using DiceRoller.Core.Exceptions;
using Microsoft.AspNetCore.SignalR;

namespace DiceRollerServer.Hubs
{
    public class RollHub : Hub
	{
		private readonly IRollService _rollService;
		private readonly IPartyService _partyService;
		private readonly IMapService _mapService;

        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rollService"></param>
        /// <param name="partyService"></param>
        /// <param name="mapService"></param>
        public RollHub(IRollService rollService, IPartyService partyService, IMapService mapService)
		{
			_rollService = rollService;
			_partyService = partyService;
			_mapService = mapService;
		}
        #endregion

        #region Chat and rolls
        /// <summary>
        /// Send message through chat
        /// </summary>
        /// <param name="partyIdStr"></param>
        /// <param name="userIdStr"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string partyIdStr, string userIdStr, string message)
		{
            var partyId = GuardAgainstNonIntParam(partyIdStr);
            if (!Int32.TryParse(userIdStr, out int userId) && userIdStr.ToUpper() != "GM")
				return;
            await Clients.Group(partyIdStr).SendAsync("ReceiveMessage", _partyService.GetUserName(partyId, userId), message);
		}

        /// <summary>
        /// Roll a die, displayed in chat
        /// </summary>
        /// <param name="partyIdStr"></param>
        /// <param name="userIdStr"></param>
        /// <param name="die"></param>
        /// <returns></returns>
        public async Task RollDie(string partyIdStr, string userIdStr, int die)
        {
            var partyId = GuardAgainstNonIntParam(partyIdStr);

            if (!Int32.TryParse(userIdStr, out int userId) && userIdStr.ToUpper() != "GM")
                return;

            int result = _rollService.RollDie(userId, die);
            await Clients.Group(partyIdStr).SendAsync("ReceiveRollResult", _partyService.GetUserName(partyId, userId), result, die);
        }

        #endregion

        #region Session management
        /// <summary>
        /// Create session as GM
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task CreateSession(string name, string password)
		{
			try
			{
				var party = _partyService.CreateSession(name, password);
				_mapService.LoadPartyMaps(party.ID);
				await Groups.AddToGroupAsync(Context.ConnectionId, "Party_"+party.ID);
				await Clients.Caller.SendAsync("SessionCreated", party.ID);
				Context.User.AddIdentity(new System.Security.Claims.ClaimsIdentity("RollHubId", "Party_"+party.ID, "GM"));
			}
			catch
			{
				await Clients.Caller.SendAsync("FailedToCreateSession");
			}
		}

        /// <summary>
        /// Try to connect player to a session
        /// </summary>
        /// <param name="name"></param>
        /// <param name="partyIdStr"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task ConnectPlayer(string name, string partyIdStr, string password)
        {
            var partyId = GuardAgainstNonIntParam(partyIdStr);
            
            if (!ValidateInfo(name))
                await Clients.Caller.SendAsync("FailedToJoinSession", "Invalid parameters");

            var userId = _partyService.Connect(partyId, name, password);
            if (userId >= 1)
            {
                Context.User.AddIdentity(new System.Security.Claims.ClaimsIdentity("RollHubId", "Party_"+ partyId + "_GM", "Player"+userId));
                await Clients.Caller.SendAsync("CanJoinSessionAsPlayer", partyId, userId, name);
                return;
            }

            await Clients.Caller.SendAsync("FailedToJoinSession", "Session not available");
        }

        private bool ValidateInfo(string name)
        {
            if (name.ToUpper() == "GM")
                return false;

            return true;
        }

        /// <summary>
        /// Initialize session for GM
        /// </summary>
        /// <param name="partyIdStr"></param>
        /// <returns></returns>
        public async Task JoinGm(string partyIdStr)
        {
            try
            {
                var partyId = GuardAgainstNonIntParam(partyIdStr);

                await Groups.AddToGroupAsync(Context.ConnectionId, partyIdStr);
                await Clients.OthersInGroup(partyId.ToString()).SendAsync("JoinedUser", partyId, "GM");
                await Clients.Caller.SendAsync("UpdateMapList", GetMaps(partyId));
            }
            catch(Exception ex)
            {
                await Clients.Caller.SendAsync("FailedToJoinSession");
                return;
            }
        }

        /// <summary>
        /// Initialize session for player
        /// </summary>
        /// <param name="partyIdStr"></param>
        /// <param name="userIdStr"></param>
        /// <returns></returns>
        public async Task JoinPlayer(string partyIdStr, string userIdStr)
        {
            var partyId = GuardAgainstNonIntParam(partyIdStr);

            int userId = 0;

            if (userIdStr.ToUpper() != "GM" && !Int32.TryParse(userIdStr, out userId))
            {
                await Clients.Caller.SendAsync("FailedToJoinSession");
                return;
            }

            string name = _partyService.GetUserName(partyId, userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, partyIdStr);
            if (userId >= 1)
            {
                await Clients.Caller.SendAsync("JoinSessionAsPlayer", partyId, userId, name);
                await Clients.OthersInGroup(partyId.ToString()).SendAsync("JoinedUser", partyId, name);
                await RaiseUserListUpdate(partyId.ToString());
                return;
            }
        }

        #endregion

        #region general info
        /// <summary>
        /// Get party name
        /// </summary>
        /// <param name="partyIdStr"></param>
        /// <returns></returns>
        public string GetPartyName(string partyIdStr)
		{
            var partyId = GuardAgainstNonIntParam(partyIdStr);
            return _partyService.GetParty(partyId).Name;
		}

        /// <summary>
        /// Get user name
        /// </summary>
        /// <param name="partyIdStr"></param>
        /// <param name="userIdStr"></param>
        /// <returns></returns>
		public string GetUser(string partyIdStr, string userIdStr)
		{
            var partyId = GuardAgainstNonIntParam(partyIdStr);
            var userId = GuardAgainstNonIntParam(userIdStr);

            return _partyService.GetUserName(partyId, userId);
		}
        #endregion

        #region user list management
        /// <summary>
        /// Raise user list update
        /// </summary>
        /// <param name="partyId"></param>
        /// <returns></returns>
        private async Task RaiseUserListUpdate(string partyId)
        {
            await Clients.Groups(partyId).SendAsync("UsersUpdated", GetUsers(partyId));
        }

        /// <summary>
        /// Get user list
        /// </summary>
        /// <param name="partyIdStr"></param>
        /// <returns></returns>
        private string GetUsers(string partyIdStr)
		{
            var res = "";

			if (Int32.TryParse(partyIdStr, out int partyId))
			{
				var users = _partyService.GetUsers(partyId);
				foreach (var user in users)
					res += user.ID + ";" + user.Name + ";" + user.HpMax + ";" + user.Hp+";"+user.ImageUrl+";"+user.Position.X+";"+user.Position.Y+"|";
				if (users.Count >0)
                    res = res.Substring(0, res.Length - 1);
			}

			return res;
		}
        
        /// <summary>
        /// Update character HP
        /// </summary>
        /// <param name="partyIdStr"></param>
        /// <param name="userIdStr"></param>
        /// <param name="hpStr"></param>
        public async void UpdateHp(string partyIdStr, string userIdStr, string hpStr)
        {
            var partyId = GuardAgainstNonIntParam(partyIdStr);
            var userId = GuardAgainstNonIntParam(userIdStr);
            var hp = GuardAgainstNonIntParam(hpStr);

            _partyService.UpdateHp(partyId, userId, hp);
            await RaiseUserListUpdate(partyIdStr);
        }
        #endregion

        #region Map management
        /// <summary>
        /// Move token
        /// </summary>
        /// <param name="partyIdStr"></param>
        /// <param name="userIdStr"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public async Task MoveToken(string partyIdStr, string userIdStr, string X, string Y)
		{
			var partyId = GuardAgainstNonIntParam(partyIdStr);
			var userId = GuardAgainstNonIntParam(userIdStr);
			var x = GuardAgainstNonIntParam(X);
			var y = GuardAgainstNonIntParam(Y);

            if (_partyService.MoveUser(partyId, userId, new System.Drawing.Point(x, y)))
				await Clients.OthersInGroup(partyId.ToString()).SendAsync("ImageMoved", userId, X, Y);
			else
			{
				var position = _partyService.GetUserPosition(partyId, userId);
				await Clients.Caller.SendAsync("ImageMoved", userId, position.X, position.Y);
			}
		}

        /// <summary>
        /// Load background
        /// </summary>
        /// <param name="partyIdStr"></param>
        /// <param name="mapIdStr"></param>
        /// <returns></returns>
		public async Task LoadBackground(string partyIdStr, string mapIdStr)
		{
			var partyId = GuardAgainstNonIntParam(partyIdStr);
			var mapId = GuardAgainstNonIntParam(mapIdStr);

            var map = _mapService.GetMap(partyId, mapId);
			await Clients.Group(partyId.ToString()).SendAsync("UpdatedBackground", map.Url, map.TileNumber);
		}

        /// <summary>
        /// Get list of maps
        /// </summary>
        /// <param name="partyId"></param>
        /// <returns></returns>
		public string GetMaps(int partyId)
		{
			var maps = _mapService.GetMapList(partyId);
			var result = "";
			foreach(var map in maps)
				result += map.ID + ";" + map.Name + "|";
			return result.Substring(0, result.Length - 1);
        }
        #endregion

        /// <summary>
        /// Check parameter value
        /// </summary>
        /// <param name="valueStr"></param>
        /// <returns></returns>
        /// <exception cref="InvalidArgumentException"></exception>
        private int GuardAgainstNonIntParam(string valueStr)
		{
            if (valueStr != null && Int32.TryParse(valueStr, out var value))
                return value;

			throw new InvalidArgumentException(typeof(int), valueStr);
        }
    }
}
