using DiceRollerServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceRoller.Core.Models
{
    public class MapPack
    {
        private readonly List<Map> _maps;

        public int ID { get; set; }

        public IReadOnlyList<Map> Maps { get { return _maps; } }

        public MapPack(int id)
        {
            ID = id;
            _maps = new List<Map>();
        }

        public void AddMap(Map map)
        {
            _maps.Add(map);
        }
    }
}
