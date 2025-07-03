// LeaveLobbyTests/GivenUserInLobby.cs
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.LeaveLobbyTests
{
    public class GivenUserInLobby : WhenTestingLeaveLobby
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ConnectionIdIsSet)
            .And(UserJoinsLobby)
            .When(LeaveLobbyIsCalled)
            .Then(ConnectionShouldBeRemoved);

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
    }
}