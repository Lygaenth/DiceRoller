namespace DiceRollerServer.Models
{
    public class PartyMember
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }

        public PartyMember(int id)
        {
            ID = id;
            HP = 10;
        }

    }
}
