using System.Linq;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.GetStateTests
{
    public class GivenLobbyDoesNotExist : WhenTestingGetState
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .When(GetStateIsCalled)
            .Then(ShouldReturnDefaultState)
            .And(ShouldCreateNewLobby);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "NEW123";
        }

        [Then]
        public void ShouldReturnDefaultState()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Creatures, Is.Empty);
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(0));
            Assert.That(Result.CurrentTurn, Is.EqualTo(0));
            Assert.That(Result.CurrentMode, Is.EqualTo(LobbyMode.Waiting));
        }

        [Then]
        public void ShouldCreateNewLobby()
        {
            // Verify that the lobby was created by calling GetState again
            var secondResult = LobbyStateManager.GetState(RoomCode);
            Assert.That(secondResult, Is.Not.Null);
            Assert.That(secondResult.CurrentMode, Is.EqualTo(LobbyMode.Waiting));
        }
    }
}