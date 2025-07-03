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
    public class GivenValidRoomCode : WhenTestingJoinLobby
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ConnectionIdIsSet)
            .And(RoomCodeIsSet)
            .And(RoomExists)
            .When(JoinLobbyIsCalled)
            .Then(ShouldReturnSuccess);

        [Given]
        public void ConnectionIdIsSet()
        {
            ConnectionId = Guid.NewGuid().ToString();
        }

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "TEST123";
        }

        [Given]
        public void RoomExists()
        {
            InitiativeUserRepository.RoomCodeExists(RoomCode, CancellationToken)
                .Returns(Task.FromResult(true));
        }

        [Then]
        public void ShouldReturnSuccess()
        {
            Assert.That(Result.success, Is.True);
            Assert.That(Result.error, Is.EqualTo(LobbyServiceError.None));
        }
    }
}