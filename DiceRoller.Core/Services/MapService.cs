using DiceRoller.Core.Apis;
using DiceRoller.Core.Models;
using DiceRoller.Core.Models.Base;
using DiceRollerServer.Models;

namespace DiceRollerServer.Services
{
    /// <summary>
    /// Map service
    /// </summary>
    public class MapService : IMapService
    {
        private readonly ElementList<MapPack> _mapsPack;

        /// <summary>
        /// Constructor
        /// </summary>
        public MapService()
        {
            _mapsPack = new ElementList<MapPack>();
        }

        /// <summary>
        /// Load party maps
        /// </summary>
        /// <param name="partyId"></param>
        public void LoadPartyMaps(int partyId)
        {
            var pack = new MapPack(partyId);
            // TODO: load from a database
            pack.AddMap(new Map(1, "./../../Media/Maps/Slime_cave.png", "Slime cave", 16));
            pack.AddMap(new Map(2, "./../../Media/Maps/Slime_cave_2.png", "Slime cave 2", 16));
            pack.AddMap(new Map(3, "./../../Media/Maps/Inn.png", "Inn", 13));
            _mapsPack.Add(pack);
        }

        /// <summary>
        /// Get map
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Map GetMap(int partyId, int id)
        {
            return _mapsPack.GetFirst(partyId).Maps.GetFirst(id);
        }

        public List<Map> GetMapList(int partyId)
        {
            return _mapsPack.GetFirst(partyId).Maps.ToList();
        }
    }
}
