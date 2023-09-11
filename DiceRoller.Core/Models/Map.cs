using DiceRoller.Core.Models.Base;

namespace DiceRollerServer.Models
{
    public class Map : Element
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public int TileNumber { get; set; }

        public Map(int id, string url, string name, int tileNumber)
            : base(id)
        {
            Url = url;
            Name = name;
            TileNumber = tileNumber;
        }
    }
}
