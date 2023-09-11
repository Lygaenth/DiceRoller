using DiceRoller.Core.Apis;
using DiceRollerServer.Models;
using System.Drawing;

namespace DiceRollerServer.Services
{
    public class PartyService : IPartyService
    {
        private readonly Dictionary<int, Party> _parties;

        public PartyService()
        {
            _parties = new Dictionary<int, Party>();
        }

        public Party CreateSession(string name, string password)
        {
            int id = 1;
            var seedGenerator = new Random();
            id = seedGenerator.Next(1000, 9999);
            while (_parties.ContainsKey(id))
                id = seedGenerator.Next(1000, 9999);

            var party = new Party(id, name);
            _parties[id] = party;

            var gm = new GameMaster(party, password);
            party.RegisterGm(gm);
            return party;
        }

        public Party GetParty(int id)
        {
            if (_parties.ContainsKey(id))
                return _parties[id];
            throw new Exception("invalid party id");
        }

        public int Connect(int partyId, string name, string password)
        {
            if (!_parties.ContainsKey(partyId))
                return -1;

            var party = _parties[partyId];
            var member = new PartyMember(party.GetFirstAvailablePlayerId());
            member.Name = name;
            if (party.Register(member, password))
                return member.ID;
            else
                return -1;
        }

        public string GetUserName(int partyId, int userId)
        {
            if (userId == 0)
                return "GM";

            return _parties[partyId].Members.First(m => m.ID == userId).Name;
        }

        public List<PartyMember> GetUsers(int partyId)
        {
            return _parties[partyId].Members;
        }

        public bool MoveUser(int partyId, int userId, Point position)
        {
            // check position is in map
            if (position.X <= 0 || position.Y <= 0)
                return false;

            _parties[partyId].Members.First(m => m.ID == userId).Move(position.X, position.Y);
            return true;
        }

        public Point GetUserPosition(int partyId, int userId)
        {
            return _parties[partyId].Members[userId].Position;
        }
    }
}
