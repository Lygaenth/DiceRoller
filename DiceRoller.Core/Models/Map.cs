namespace DiceRollerServer.Models
{
    public class Map
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public int TileNumber { get; set; }

        public Map(int id, string url, string name, int tileNumber)
        {
            Id = id;
            Url = url;
            Name = name;
            TileNumber = tileNumber;
        }
    }
}
