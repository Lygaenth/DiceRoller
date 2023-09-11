using DiceRoller.Core.Models.Base;

namespace DiceRollerServer.Models
{
    public class GameMaster : Element
    {
        Party Party { get; }
        public string Password { get; set; }

        public GameMaster(int id, Party party, string password)
            : base(id)
        {
            Party = party;
            Password = password;
        }
    }
}
