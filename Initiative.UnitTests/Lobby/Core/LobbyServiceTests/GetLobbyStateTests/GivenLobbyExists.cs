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
    public class GivenLobbyExists : WhenTestingGetLobbyState
    {
        protected string ConnectionId;
        protected EncounterDto InitialState;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(LobbyIsCreated)
            .And(InitialStateIsSet)
            .When(GetLobbyStateIsCalled)
            .Then(ShouldReturnCorrectState);

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
        public async Task InitialStateIsSet()
        {
            InitialState = new EncounterDto
            {
                Creatures = new[] { "Creature1", "Creature2" },
                CurrentCreatureIndex = 1,
                CurrentTurn = 2,
                CurrentMode = LobbyMode.InProgress
            };
            await LobbyService.SetLobbyState(RoomCode, InitialState);
        }

        [Then]
        public void ShouldReturnCorrectState()
        {
            Assert.That(Result.CurrentMode, Is.EqualTo(InitialState.CurrentMode));
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(InitialState.CurrentCreatureIndex));
            Assert.That(Result.CurrentTurn, Is.EqualTo(InitialState.CurrentTurn));
            Assert.That(Result.Creatures, Is.EquivalentTo(InitialState.Creatures));
        }
    }
}
