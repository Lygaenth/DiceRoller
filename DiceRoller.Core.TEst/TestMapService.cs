using DiceRoller.Core.Exceptions;
using DiceRollerServer.Services;
using Xunit;

namespace DiceRoller.Core.TEst
{
    public class TestMapService
    {

        [Fact]
        public void TestLoadMaps()
        {
            int userId = 2;
            var mapService = new MapService();
            mapService.LoadPartyMaps(userId);
            Assert.True(mapService.GetMapList(userId) != null);
        }

        [Fact]
        public void TestGetMap()
        {
            int userId = 2;
            var mapService = new MapService();
            mapService.LoadPartyMaps(userId);
            var mapList = mapService.GetMapList(userId);
            Assert.Equal(3, mapList.Count);
            mapList[0].Name = "Slime cave";
        }

        [Fact]
        public void TestInvalidMapPackId()
        {
            int userId = 2;
            var mapService = new MapService();
            mapService.LoadPartyMaps(userId);
            Assert.Throws<UnknownIdException>(() => mapService.GetMapList(1));
        }

        [Fact]
        public void TestInvalidMapId()
        {
            int userId = 2;
            var mapService = new MapService();
            mapService.LoadPartyMaps(userId);
            Assert.Throws<UnknownIdException>(() => mapService.GetMap(2, 10));
        }
    }
}