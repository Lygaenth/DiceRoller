using DiceRoller.Core.Apis;
using DiceRoller.Core.Models;
using DiceRoller.Core.Models.Base;
using DiceRollerServer.Models;
using System.Drawing;

namespace DiceRollerServer.Services
{
    public class PartyService : IPartyService
    {
        private readonly ElementList<Party> _parties;

        /// <summary>
        /// Constructor
        /// </summary>
        public PartyService()
        {
            _parties = new ElementList<Party>();
        }

        /// <summary>
        /// Create session
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Party CreateSession(string name, string password)
        {
            int id = GenerateSeed();

            var party = new Party(id, name);
            _parties.Add(party);

            var gm = new GameMaster(id, party, password);
            party.RegisterGm(gm);
            return party;
        }

        /// <summary>
        /// Generate seed for party
        /// </summary>
        /// <returns></returns>
        private int GenerateSeed()
        {
            var id = 1;
            var seedGenerator = new Random();
            id = seedGenerator.Next(1000, 9999);
            while (_parties.Any(p => p.ID == id))
                id = seedGenerator.Next(1000, 9999);
            return id;
        }

        /// <summary>
        /// Get party
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Party GetParty(int id)
        {
            return _parties.GetFirst(id);
        }

        /// <summary>
        /// Connect to party session
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int Connect(int partyId, string name, string password)
        {
            if (!_parties.Any(p => p.ID == partyId))
                return -1;

            var party = _parties.GetFirst(partyId);
            var member = new PartyMember(party.GetFirstAvailablePlayerId());
            member.Name = name;
            if (party.Register(member, password))
                return member.ID;
            else
                return -1;
        }

        /// <summary>
        /// Get user name
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserName(int partyId, int userId)
        {
            if (userId == 0)
                return "GM";

            return _parties.GetFirst(partyId).Members.GetFirst(userId).Name;
        }

        /// <summary>
        /// Get users
        /// </summary>
        /// <param name="partyId"></param>
        /// <returns></returns>
        public List<PartyMember> GetUsers(int partyId)
        {
            return _parties.GetFirst(partyId).Members;
        }

        /// <summary>
        /// Move user
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="userId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool MoveUser(int partyId, int userId, Point position)
        {
            // check position is in map
            if (position.X <= 0 || position.Y <= 0)
                return false;

            _parties.GetFirst(partyId).Members.GetFirst(userId).Move(position.X, position.Y);
            return true;
        }

        /// <summary>
        /// Get user position
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Point GetUserPosition(int partyId, int userId)
        {
            return _parties.GetFirst(partyId).Members.GetFirst(userId).Position;
        }

        /// <summary>
        /// Update user's character HP
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="userId"></param>
        /// <param name="hp"></param>
        /// <returns></returns>
        public bool UpdateHp(int partyId, int userId, int hp)
        {
            var user = _parties.GetFirst(partyId).Members.GetFirst(userId);
            if (hp > user.HpMax)
                return false;

            user.Hp = hp;
            return true;
        }
    }
}
