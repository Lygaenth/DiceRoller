using DiceRoller.Core.Apis;
using DiceRoller.Core.Exceptions;
using DiceRollerServer.Services;

namespace DiceRoller.Core.TEst
{
    public class TestRollService
    {
        [Theory]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(12)]
        [InlineData(20)]
        void TestRollDie(int die)
        {
            IRollService rollService = new RollService();
            Assert.True(rollService.RollDie(1, die) <= die);
        }

        [Fact]
        void TestRollsStat()
        {
            int userId = 3;
            IRollService rollService = new RollService();
            rollService.Connect(userId);
            var rolls = new List<int>();
            rolls.Add(rollService.RollDie(userId, 20));
            rolls.Add(rollService.RollDie(userId, 20));
            rolls.Add(rollService.RollDie(userId, 20));            
            Assert.Equal(rolls.Sum() / 3, rollService.GetRollStat(userId, 20));
        }

        [Fact]
        void TestConnectPlayer()
        {
            int userId = 3;
            IRollService rollService = new RollService();
            rollService.Connect(userId);
            Assert.Equal(-1, rollService.GetRollStat(userId, 20));
        }

        [Fact]
        void TestNotconnectedPlayerException()
        {
            int userId = 3;
            IRollService rollService = new RollService();
            Assert.Throws<UnknownIdException>(() => rollService.GetRollStat(userId, 20));
        }
    }
}
