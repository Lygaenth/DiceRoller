namespace DiceRollerServer.Models
{
    public class PartyMember
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Hp { get; set; }
        public int HpMax { get; set; }
        public PartyMember(int id)
        {
            ID = id;
            HpMax = 10;
            Hp = 10;
        }

    }
}
