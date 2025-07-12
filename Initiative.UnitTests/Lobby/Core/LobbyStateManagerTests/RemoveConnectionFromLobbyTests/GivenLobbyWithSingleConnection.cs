using System;
using System.Linq;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.RemoveConnectionFromLobbyTests
{
    public class GivenLobbyWithSingleConnection : WhenTestingRemoveConnectionFromLobby
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(LobbyWithSingleConnectionExists)
            .When(RemoveConnectionFromLobbyIsCalled)
            .Then(ShouldRemoveConnectionAndLobby);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "SINGLE123";
        }

        [Given]
        public void LobbyWithSingleConnectionExists()
        {
            ConnectionId = "only-connection";
            LobbyStateManager.AddConnectionToLobby(RoomCode, ConnectionId);
        }

        [Then]
        public void ShouldRemoveConnectionAndLobby()
        {
            // Verify lobby was removed since no connections remain
            Assert.That(LobbyStateManager.LobbyExists(RoomCode), Is.False);
        }
    }
}