using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Dtos;
using Initiative.Lobby.Core.Services;
using Initiative.Persistence.Models.Lobby;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetLobbyStateTests
{
    public class GivenStoredStateRecoveredThenCalledAgain : WhenTestingGetLobbyState
    {
        protected LobbyStateDto StoredState;
        protected EncounterDto SecondResult;
        protected EncounterDto RecoveredState;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(StoredStateExists)
            .And(LobbyStateManagerSetup)
            .When(GetLobbyStateIsCalledTwice)
            .Then(ShouldReturnStoredState)
            .And(SecondCallShouldReturnSameState)
            .And(ShouldOnlyCallRepositoryOnce);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "RECOVERY123";
        }

        [Given]
        public void StoredStateExists()
        {
            StoredState = new LobbyStateDto
            {
                Id = "test-id",
                RoomCode = RoomCode,
                Creatures = new[]
                {
                    new LobbyCreatureStateDto { DisplayName = "RecoveredCreature1", Statuses = ["Focused"], HealthStatus = "Healthy", IsPlayer = false, IsHidden = false },
                    new LobbyCreatureStateDto { DisplayName = "RecoveredCreature2", Statuses = [], HealthStatus = "Bloodied", IsPlayer = true, IsHidden = false }
                },
                TurnNumber = 3,
                CurrentCreatureIndex = 1,
                CurrentMode = LobbyMode.InProgress
            };

            LobbyStateRepository.FetchLobbyStateByRoomCode(RoomCode, CancellationToken)
                .Returns(StoredState);
        }

        [Given]
        public void LobbyStateManagerSetup()
        {
            RecoveredState = new EncounterDto
            {
                Creatures = StoredState.Creatures.Select(creature => new LobbyCreatureDto
                {
                    DisplayName = creature.DisplayName,
                    Statuses = creature.Statuses,
                    HealthStatus = creature.HealthStatus,
                    IsPlayer = creature.IsPlayer,
                    IsHidden = creature.IsHidden
                }),
                CurrentCreatureIndex = StoredState.CurrentCreatureIndex,
                CurrentTurn = StoredState.TurnNumber,
                CurrentMode = StoredState.CurrentMode
            };

            // First call: lobby not in memory, second call: lobby exists in memory
            LobbyStateManager.LobbyExists(RoomCode)
                .Returns(false, true); // First call false, second call true

            LobbyStateManager.GetState(RoomCode).Returns(RecoveredState);
        }

        [When]
        public async Task GetLobbyStateIsCalledTwice()
        {
            await GetLobbyStateIsCalled();
            SecondResult = await LobbyService.GetLobbyState(RoomCode);
        }

        [Then]
        public void ShouldReturnStoredState()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Creatures.Select(c => c.DisplayName), Is.EquivalentTo(StoredState.Creatures.Select(c => c.DisplayName)));
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(StoredState.CurrentCreatureIndex));
            Assert.That(Result.CurrentTurn, Is.EqualTo(StoredState.TurnNumber));
            Assert.That(Result.CurrentMode, Is.EqualTo(StoredState.CurrentMode));
        }

        [Then]
        public void SecondCallShouldReturnSameState()
        {
            Assert.That(SecondResult, Is.Not.Null);
            Assert.That(SecondResult.Creatures.Select(c => c.DisplayName), Is.EquivalentTo(Result.Creatures.Select(c => c.DisplayName)));
            Assert.That(SecondResult.CurrentCreatureIndex, Is.EqualTo(Result.CurrentCreatureIndex));
            Assert.That(SecondResult.CurrentTurn, Is.EqualTo(Result.CurrentTurn));
            Assert.That(SecondResult.CurrentMode, Is.EqualTo(Result.CurrentMode));
        }

        [Then]
        public async Task ShouldOnlyCallRepositoryOnce()
        {
            // Repository should only be called once (first call), as the second call uses LobbyStateManager
            await LobbyStateRepository.Received(1).FetchLobbyStateByRoomCode(RoomCode, CancellationToken);
        }
    }
}