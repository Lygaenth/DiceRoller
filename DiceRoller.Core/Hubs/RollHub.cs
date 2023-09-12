using DiceRoller.Core.Apis;
using DiceRoller.Core.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace DiceRollerServer.Hubs
{
    public class RollHub : Hub
	{
		private readonly IRollService _rollService;
		private readonly IPartyService _partyService;
		private readonly IMapService _mapService;
        private readonly Logger _logger;
        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rollService"></param>
        /// <param name="partyService"></param>
        /// <param name="mapService"></param>
        public RollHub(IRollService rollService, IPartyService partyService, IMapService mapService, Logger logger)
		{
			_rollService = rollService;
			_partyService = partyService;
			_mapService = mapService;
            _logger = logger;
            _logger.Information("Hub built");
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
            var partyId = Guards.AgainstNonIntParam(partyIdStr);
            if (!Int32.TryParse(userIdStr, out int userId) && userIdStr.ToUpper() != "GM")
            {
                _logger.Error("Failed to send message in party {0} for {1} ", partyIdStr, userIdStr);
                return;
            }

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
            var partyId = Guards.AgainstNonIntParam(partyIdStr);

            if (!Int32.TryParse(userIdStr, out int userId) && userIdStr.ToUpper() != "GM")
            {
                _logger.Error("Failed to roll die party " + partyIdStr + " for user " + userIdStr);
                return;
            }

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
                _logger.Information("Created session "+party.ID+": "+name);
			}
			catch(Exception ex)
			{
                _logger.Error("Failed to create session: "+ex.Message);
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
            var partyId = Guards.AgainstNonIntParam(partyIdStr);

            if (!ValidateInfo(name))
            {
                await Clients.Caller.SendAsync("FailedToJoinSession", "Invalid name or password");
            }

            var userId = _partyService.Connect(partyId, name, password);
            if (userId >= 1)
            {
                Context.User.AddIdentity(new System.Security.Claims.ClaimsIdentity("RollHubId", "Party_"+ partyId + "_GM", "Player"+userId));
                await Clients.Caller.SendAsync("CanJoinSessionAsPlayer", partyId, userId, name);
                return;
            }

            _logger.Information("User failed to connect to session " + partyId);
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
                var partyId = Guards.AgainstNonIntParam(partyIdStr);

                await Groups.AddToGroupAsync(Context.ConnectionId, partyIdStr);
                await Clients.OthersInGroup(partyId.ToString()).SendAsync("JoinedUser", partyId, "GM");
                await Clients.Caller.SendAsync("UpdateMapList", GetMaps(partyId));
            }
            catch(Exception ex)
            {
                _logger.Error("GM connection to session failed: " + ex.Message);
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
            var partyId = Guards.AgainstNonIntParam(partyIdStr);

            int userId = 0;

            if (userIdStr.ToUpper() != "GM" && !Int32.TryParse(userIdStr, out userId))
            {
                _logger.Error("Failed to join session "+partyIdStr+" for user "+userIdStr);
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
            var partyId = Guards.AgainstNonIntParam(partyIdStr);
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
            var partyId = Guards.AgainstNonIntParam(partyIdStr);
            var userId = Guards.AgainstNonIntParam(userIdStr);

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
            var partyId = Guards.AgainstNonIntParam(partyIdStr);
            var userId = Guards.AgainstNonIntParam(userIdStr);
            var hp = Guards.AgainstNonIntParam(hpStr);

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
			var partyId = Guards.AgainstNonIntParam(partyIdStr);
			var userId = Guards.AgainstNonIntParam(userIdStr);
			var x = Guards.AgainstNonIntParam(X);
			var y = Guards.AgainstNonIntParam(Y);

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
			var partyId = Guards.AgainstNonIntParam(partyIdStr);
			var mapId = Guards.AgainstNonIntParam(mapIdStr);

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

        
    }
}
