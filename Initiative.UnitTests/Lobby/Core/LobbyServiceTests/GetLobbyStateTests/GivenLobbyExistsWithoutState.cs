using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Dtos;
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
        protected EncounterDto DefaultState;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(LobbyIsCreated)
            .And(LobbyStateManagerIsSetup)
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

        [Given]
        public void LobbyStateManagerIsSetup()
        {
            DefaultState = new EncounterDto
            {
                Creatures = Enumerable.Empty<string>(),
                CurrentCreatureIndex = 0,
                CurrentTurn = 0,
                CurrentMode = LobbyMode.Waiting
            };

            // Mock the LobbyStateManager to return true for LobbyExists and default state
            LobbyStateManager.LobbyExists(RoomCode).Returns(true);
            LobbyStateManager.GetState(RoomCode).Returns(DefaultState);
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
