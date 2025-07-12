using System;
using System.Linq;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.RemoveConnectionFromLobbyTests
{
    public class GivenLobbyWithMultipleConnections : WhenTestingRemoveConnectionFromLobby
    {
        protected string RemainingConnectionId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(LobbyWithMultipleConnectionsExists)
            .When(RemoveConnectionFromLobbyIsCalled)
            .Then(ShouldRemoveConnectionButKeepLobby);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "MULTI123";
        }

        [Given]
        public void LobbyWithMultipleConnectionsExists()
        {
            ConnectionId = "connection-to-remove";
            RemainingConnectionId = "remaining-connection";
            
            LobbyStateManager.AddConnectionToLobby(RoomCode, ConnectionId);
            LobbyStateManager.AddConnectionToLobby(RoomCode, RemainingConnectionId);
        }

        [Then]
        public void ShouldRemoveConnectionButKeepLobby()
        {
            // Verify lobby still exists
            Assert.That(LobbyStateManager.LobbyExists(RoomCode), Is.True);
            
            // Verify specific connection was removed but other remains
            var lobbyState = LobbyStateManager.GetLobbyState(RoomCode);
            var connections = lobbyState.GetConnections();
            Assert.That(connections, Does.Not.Contain(ConnectionId));
            Assert.That(connections, Contains.Item(RemainingConnectionId));
            Assert.That(connections.Count(), Is.EqualTo(1));
        }
    }
}