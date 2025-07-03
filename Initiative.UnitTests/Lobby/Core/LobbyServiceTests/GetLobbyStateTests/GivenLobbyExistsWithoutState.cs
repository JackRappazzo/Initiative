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

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetLobbyStateTests
{
    public class GivenLobbyExistsWithoutState : WhenTestingGetLobbyState
    {
        protected string ConnectionId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(LobbyIsCreated)
            .When(GetLobbyStateIsCalled)
            .Then(ShouldReturnDefaultState);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "TEST123";
            ConnectionId = Guid.NewGuid().ToString();
        }

        [Given]
        public async Task LobbyIsCreated()
        {
            InitiativeUserRepository.RoomCodeExists(RoomCode, CancellationToken)
                .Returns(Task.FromResult(true));
            await LobbyService.JoinLobby(ConnectionId, RoomCode, CancellationToken);
        }

        [Then]
        public void ShouldReturnDefaultState()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Creatures, Is.Not.Null);
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(0));
            Assert.That(Result.CurrentTurn, Is.EqualTo(0));
            Assert.That(Result.CurrentMode, Is.EqualTo(LobbyMode.Waiting));
        }
    }
}
