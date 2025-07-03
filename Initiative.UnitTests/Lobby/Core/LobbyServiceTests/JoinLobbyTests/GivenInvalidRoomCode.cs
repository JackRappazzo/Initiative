using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.JoinLobbyTests
{
    public class GivenInvalidRoomCode : WhenTestingJoinLobby
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ConnectionIdIsSet)
            .And(RoomCodeIsSet)
            .And(RoomDoesNotExist)
            .When(JoinLobbyIsCalled)
            .Then(ShouldReturnRoomNotFound);

        [Given]
        public void ConnectionIdIsSet()
        {
            ConnectionId = Guid.NewGuid().ToString();
        }

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "INVALID";
        }

        [Given]
        public void RoomDoesNotExist()
        {
            InitiativeUserRepository.RoomCodeExists(RoomCode, CancellationToken)
                .Returns(Task.FromResult(false));
        }

        [Then]
        public void ShouldReturnRoomNotFound()
        {
            Assert.That(Result.success, Is.False);
            Assert.That(Result.error, Is.EqualTo(LobbyServiceError.RoomNotFound));
        }
    }
}
