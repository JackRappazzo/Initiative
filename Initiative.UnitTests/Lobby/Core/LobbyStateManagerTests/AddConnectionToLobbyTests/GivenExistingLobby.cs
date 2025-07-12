using System;
using System.Linq;
using Initiative.Lobby.Core.Dtos;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.AddConnectionToLobbyTests
{
    public class GivenExistingLobby : WhenTestingAddConnectionToLobby
    {
        protected string ExistingConnectionId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(LobbyAlreadyExists)
            .And(NewConnectionIdIsSet)
            .When(AddConnectionToLobbyIsCalled)
            .Then(ShouldAddConnectionToExistingLobby);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "EXISTING123";
        }

        [Given]
        public void LobbyAlreadyExists()
        {
            ExistingConnectionId = "existing-connection";
            LobbyStateManager.AddConnectionToLobby(RoomCode, ExistingConnectionId);
        }

        [Given]
        public void NewConnectionIdIsSet()
        {
            ConnectionId = "new-connection";
        }

        [Then]
        public void ShouldAddConnectionToExistingLobby()
        {
            // Verify lobby exists
            Assert.That(LobbyStateManager.LobbyExists(RoomCode), Is.True);
            
            // Verify both connections exist
            var lobbyState = LobbyStateManager.GetLobbyState(RoomCode);
            var connections = lobbyState.GetConnections();
            Assert.That(connections, Contains.Item(ExistingConnectionId));
            Assert.That(connections, Contains.Item(ConnectionId));
            Assert.That(connections.Count(), Is.EqualTo(2));
        }
    }
}