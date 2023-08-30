using DiceRollerServer.Models;

namespace DiceRollerServer.Services
{
    public class PartyService
    {
        private readonly Dictionary<int, Party> _parties;

        public PartyService()
        {
            _parties = new Dictionary<int, Party>();
        }

        public Party CreateSession(string name, string password)
        {
            int id = 1;
            while(_parties.ContainsKey(id))
                id++;

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

        public string GetUser(int partyId, int userId)
        {
            if (userId == 0)
                return "GM";

            return _parties[partyId].Members.First(m => m.ID == userId).Name;
        }

        public List<PartyMember> GetUsers(int partyId)
        {
            return _parties[partyId].Members;
        }
    

    }
}
