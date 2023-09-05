using DiceRollerServer.Services;

namespace DiceRollerServer.Models
{
    public class Party : IDisposable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public GameMaster? Gm { get; set; }
        public List<PartyMember> Members { get; }

        public Party(int id, string name)
        {
            ID = id;
            Members = new List<PartyMember>();
            Name = name;
        }

        public void RegisterGm(GameMaster gm)
        {
            if (Gm != null)
                throw new Exception("Game master already registered");

            Gm = gm;
        }

        public bool Register(PartyMember partyMember, string password)
        {
            if (Gm == null || password != Gm.Password)
                return false;

            Members.Add(partyMember);
            return true;
        }

        public int GetFirstAvailablePlayerId()
        {
            int id = 1;
            while (Members.Any(m => m.ID == id))
                id++;
            return id;
        }

        public void Dispose()
        {

        }
    }
}
