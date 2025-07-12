using System;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.RemoveConnectionFromLobbyTests
{
    public class GivenNonExistentLobby : WhenTestingRemoveConnectionFromLobby
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(ConnectionIdIsSet)
            .When(RemoveConnectionFromLobbyIsCalled)
            .Then(ShouldHandleGracefully);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "NONEXISTENT123";
        }

        [Given]
        public void ConnectionIdIsSet()
        {
            ConnectionId = "some-connection";
        }

        [Then]
        public void ShouldHandleGracefully()
        {
            // Should not throw an exception and lobby should still not exist
            Assert.That(LobbyStateManager.LobbyExists(RoomCode), Is.False);
        }
    }
}