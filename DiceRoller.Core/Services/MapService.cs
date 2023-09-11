using DiceRoller.Core.Apis;
using DiceRoller.Core.Exceptions;
using DiceRoller.Core.Models;
using DiceRollerServer.Models;

namespace DiceRollerServer.Services
{
    public class MapService : IMapService
    {
        private readonly List<MapPack> _mapsPack;
        public MapService()
        {
            _mapsPack = new List<MapPack>();
        }

        public void LoadPartyMaps(int partyId)
        {
            var pack = new MapPack(partyId);
            pack.AddMap(new Map(1, "./../../Media/Maps/Slime_cave.png", "Slime cave", 16));
            pack.AddMap(new Map(2, "./../../Media/Maps/Slime_cave_2.png", "Slime cave 2", 16));
            pack.AddMap(new Map(3, "./../../Media/Maps/Inn.png", "Inn", 13));
            _mapsPack.Add(pack);
        }

        public Map GetMap(int partyId, int id)
        {
            if (!_mapsPack.Any(m => m.ID == partyId))
                throw new UnknownIdException("MapPack", partyId);

            if (!_mapsPack.First(m => m.ID == partyId).Maps.Any(m => m.Id == id))
                throw new UnknownIdException("Map", id);

            return _mapsPack.First(m => m.ID == partyId).Maps.First(m=> m.Id == id);
        }

        public List<Map> GetMapList(int partyId)
        {
            if (!_mapsPack.Any(m => m.ID == partyId))
                throw new UnknownIdException("MapPack", partyId);

            return _mapsPack.First(p => p.ID == partyId).Maps.ToList();
        }
    }
}
