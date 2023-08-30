namespace DiceRollerServer.Models
{
    public class GameMaster
    {
        Party Party { get; }
        public string Password { get; set; }

        public GameMaster(Party party, string password)
        {
            Party = party;
            Password = password;
        }
    }
}
