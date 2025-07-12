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
                Creatures = new[] { "RecoveredCreature1", "RecoveredCreature2" },
                TurnNumber = 3,
                CurrentCreatureIndex = 1,
                CurrentMode = LobbyMode.InProgress
            };

            LobbyStateRepository.FetchLobbyStateByRoomCode(RoomCode, CancellationToken)
                .Returns(Task.FromResult(StoredState));
        }

        [Given]
        public void LobbyStateManagerSetup()
        {
            RecoveredState = new EncounterDto
            {
                Creatures = StoredState.Creatures,
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
            Assert.That(Result.Creatures, Is.EquivalentTo(StoredState.Creatures));
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(StoredState.CurrentCreatureIndex));
            Assert.That(Result.CurrentTurn, Is.EqualTo(StoredState.TurnNumber));
            Assert.That(Result.CurrentMode, Is.EqualTo(StoredState.CurrentMode));
        }

        [Then]
        public void SecondCallShouldReturnSameState()
        {
            Assert.That(SecondResult, Is.Not.Null);
            Assert.That(SecondResult.Creatures, Is.EquivalentTo(Result.Creatures));
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