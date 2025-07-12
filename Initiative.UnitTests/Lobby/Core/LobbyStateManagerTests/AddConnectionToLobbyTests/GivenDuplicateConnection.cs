using System;
using System.Linq;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.AddConnectionToLobbyTests
{
    public class GivenDuplicateConnection : WhenTestingAddConnectionToLobby
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(ConnectionIdIsSet)
            .And(ConnectionAlreadyExists)
            .When(AddConnectionToLobbyIsCalled)
            .Then(ShouldNotCreateDuplicate);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "DUPLICATE123";
        }

        [Given]
        public void ConnectionIdIsSet()
        {
            ConnectionId = "duplicate-connection";
        }

        [Given]
        public void ConnectionAlreadyExists()
        {
            LobbyStateManager.AddConnectionToLobby(RoomCode, ConnectionId);
        }

        [Then]
        public void ShouldNotCreateDuplicate()
        {
            // Verify lobby exists
            Assert.That(LobbyStateManager.LobbyExists(RoomCode), Is.True);
            
            // Verify connection exists only once
            var lobbyState = LobbyStateManager.GetLobbyState(RoomCode);
            var connections = lobbyState.GetConnections();
            Assert.That(connections, Contains.Item(ConnectionId));
            Assert.That(connections.Count(), Is.EqualTo(1), "Should not create duplicate connections");
        }
    }
}