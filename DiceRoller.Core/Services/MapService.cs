using DiceRollerServer.Models;

namespace DiceRollerServer.Services
{
    public class MapService
    {
        private readonly Dictionary<string, Map> _maps;
        public MapService()
        {
            _maps = new Dictionary<string, Map>();
            _maps.Add("./../../Media/Maps/Slime_cave.png", new Map(1, "./../../Media/Maps/Slime_cave.png", "Slime cave", 16));
            _maps.Add("./../../Media/Maps/Slime_cave_2.png", new Map(2, "./../../Media/Maps/Slime_cave_2.png", "Slime cave 2", 8));
        }

        public Map GetMap(string id)
        {
            return _maps[id];
        }
    }
}
