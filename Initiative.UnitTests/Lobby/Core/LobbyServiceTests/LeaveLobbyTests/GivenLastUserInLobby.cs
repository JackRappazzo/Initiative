// LeaveLobbyTests/GivenLastUserInLobby.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.LeaveLobbyTests
{
    public class GivenLastUserInLobby : WhenTestingLeaveLobby
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ConnectionIdIsSet)
            .And(UserJoinsLobby)
            .When(LeaveLobbyIsCalled)
            .Then(ConnectionShouldBeRemoved)
            .And(ShouldCallLobbyStateManager);

        [Given]
        public void ConnectionIdIsSet()
        {
            ConnectionId = Guid.NewGuid().ToString();
            RoomCode = "TEST123";
        }

        [Given]
        public async Task UserJoinsLobby()
        {
            InitiativeUserRepository.RoomCodeExists(RoomCode, CancellationToken)
                .Returns(Task.FromResult(true));
            
            await LobbyService.JoinLobby(ConnectionId, RoomCode, CancellationToken);
        }

        [Then]
        public void ConnectionShouldBeRemoved()
        {
            var result = LobbyService.GetRoomCodeByConnection(ConnectionId);
            Assert.That(result, Is.Empty);
        }

        [Then]
        public void ShouldCallLobbyStateManager()
        {
            // Verify that LobbyStateManager.RemoveConnectionFromLobby was called
            // The RemoveConnectionFromLobby method handles lobby removal automatically when no connections remain
            LobbyStateManager.Received(1).RemoveConnectionFromLobby(RoomCode, ConnectionId);
        }
    }
}