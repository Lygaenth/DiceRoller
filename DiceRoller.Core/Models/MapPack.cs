using DiceRoller.Core.Models.Base;
using DiceRollerServer.Models;

namespace DiceRoller.Core.Models
{
    public class MapPack : Element
    {
        private readonly ElementList<Map> _maps;

        public ElementList<Map> Maps { get { return _maps; } }

        public MapPack(int id)
            :base(id)
        {
            _maps = new ElementList<Map>();
        }

        public void AddMap(Map map)
        {
            _maps.Add(map);
        }
    }
}
