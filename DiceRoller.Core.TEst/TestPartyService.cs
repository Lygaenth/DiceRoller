using DiceRoller.Core.Apis;
using DiceRollerServer.Services;
using System.Drawing;

namespace DiceRoller.Core.TEst
{
    public class TestPartyService
    {
        const string PartyName = "Test party name";
        const string TestPassword = "test password";
        const string PlayerName = "TestPlayer";

        [Fact]
        void TestPartyInitialisation()
        {
            IPartyService partyService = new PartyService();
            var party = partyService.CreateSession(PartyName, TestPassword);
            Assert.True(party.ID > 999);
            Assert.Equal(PartyName, party.Name);
            Assert.NotNull(party.Gm);
            Assert.Equal(TestPassword, party.Gm.Password);

            Assert.True(party.Members.Count == 0);            
        }

        [Fact]
        void TestPlayerConnection()
        {
            IPartyService partyService = new PartyService();
            var party = partyService.CreateSession(PartyName, TestPassword);
            var playerId = partyService.Connect(party.ID, PlayerName, TestPassword);
            Assert.True(playerId > 0);
        }

        [Fact]
        void TestGetPlayerName()
        {
            IPartyService partyService = new PartyService();
            var party = partyService.CreateSession(PartyName, TestPassword);
            var playerId = partyService.Connect(party.ID, PlayerName, TestPassword);
            var playerName = partyService.GetUserName(party.ID, playerId);
            Assert.Equal(PlayerName, playerName);
        }

        [Fact]
        void TestGetUsers()
        {
            IPartyService partyService = new PartyService();
            var party = partyService.CreateSession(PartyName, TestPassword);
            var playerId = partyService.Connect(party.ID, PlayerName, TestPassword);
            var player2Id = partyService.Connect(party.ID, "Player2", TestPassword);
            var users = partyService.GetUsers(party.ID);
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Contains(users.First(u => u.ID == playerId), users);
            Assert.Contains(users.First(u => u.ID == player2Id), users);
        }

        [Fact]
        void TestMoveUser()
        {
            IPartyService partyService = new PartyService();
            var party = partyService.CreateSession(PartyName, TestPassword);
            var playerId = partyService.Connect(party.ID, PlayerName, TestPassword);
            var newPosition = new Point(50, 50);
            var moveAccepted = partyService.MoveUser(party.ID, playerId, newPosition);
            Assert.True(moveAccepted);
            Assert.Equal(newPosition, party.Members[0].Position);
            var wrongPosition = new Point(-60, 50);
            moveAccepted = partyService.MoveUser(party.ID, playerId, wrongPosition);
            Assert.False(moveAccepted);
        }

    }
}
