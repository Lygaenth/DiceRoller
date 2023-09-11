using System.Drawing;
using DiceRoller.Core.Models.Base;

namespace DiceRoller.Core.Models
{
    public class PartyMember : Element
    {
        public string Name { get; set; }
        public int Hp { get; set; }
        public int HpMax { get; private set; }
        public string ImageUrl { get; private set; }
        public Point Position { get; private set; }

        public PartyMember(int id)
            : base(id)
        {
            ID = id;
            HpMax = 10;
            Hp = 10;
            ImageUrl = "./../../media/Portraits/pic_default_" + id + ".png";
            Name = "character_1";
            Position = new Point(0, 0);
        }

        public void Move(int x, int y)
        {
            Position = new Point(x, y);
        }
    }
}
