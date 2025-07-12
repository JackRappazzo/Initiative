using System;
using System.Linq;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.AddConnectionToLobbyTests
{
    public class GivenNewLobby : WhenTestingAddConnectionToLobby
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(ConnectionIdIsSet)
            .When(AddConnectionToLobbyIsCalled)
            .Then(ShouldCreateLobbyAndAddConnection);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "NEWLOBBY123";
        }

        [Given]
        public void ConnectionIdIsSet()
        {
            ConnectionId = "connection-123";
        }

        [Then]
        public void ShouldCreateLobbyAndAddConnection()
        {
            // Verify lobby exists
            Assert.That(LobbyStateManager.LobbyExists(RoomCode), Is.True);
            
            // Verify connection was added
            var lobbyState = LobbyStateManager.GetLobbyState(RoomCode);
            var connections = lobbyState.GetConnections();
            Assert.That(connections, Contains.Item(ConnectionId));
            Assert.That(connections.Count(), Is.EqualTo(1));
        }
    }
}