using System.Drawing;

namespace DiceRollerServer.Models
{
    public class PartyMember
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Hp { get; private set; }
        public int HpMax { get; private set; }
        public string ImageUrl { get; private set; }
        public Point Position { get; private set; }

        public PartyMember(int id)
        {
            ID = id;
            HpMax = 10;
            Hp = 10;
            ImageUrl = "./../../media/Portraits/pic_default_"+id+".png";
            Name = "character_1";
            Position = new Point(0, 0);
        }
        
        public void Move(int x, int y)
        {
            Position = new Point(x, y);
        }
    }
}
