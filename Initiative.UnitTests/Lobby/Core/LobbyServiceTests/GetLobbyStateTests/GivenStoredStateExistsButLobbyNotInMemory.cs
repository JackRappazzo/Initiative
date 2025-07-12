using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public class GivenStoredStateExistsButLobbyNotInMemory : WhenTestingGetLobbyState
    {
        protected LobbyStateDto StoredState;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(StoredStateExists)
            .And(StateIsNotInMemory)
            .When(GetLobbyStateIsCalled)
            .Then(ShouldReturnStoredState)
            .And(ShouldCallSetLobbyState);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "STORED123";
        }

        [Given]
        public void StoredStateExists()
        {
            StoredState = new LobbyStateDto
            {
                Id = "test-id",
                RoomCode = RoomCode,
                Creatures = new[] { "StoredCreature1", "StoredCreature2" },
                TurnNumber = 5,
                CurrentCreatureIndex = 1,
                CurrentMode = LobbyMode.InProgress
            };

            LobbyStateRepository.FetchLobbyStateByRoomCode(RoomCode, CancellationToken)
                .Returns(Task.FromResult(StoredState));
        }

        [Given]
        public void StateIsNotInMemory()
        {
            // Mock LobbyStateManager to return false for LobbyExists (not in memory)
            LobbyStateManager.LobbyExists(RoomCode).Returns(false);
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
        public void ShouldCallSetLobbyState()
        {
            // Verify that LobbyStateManager.SetState was called to update the in-memory state
            LobbyStateManager.Received(1).SetState(
                RoomCode,
                Arg.Is<EncounterDto>(e => 
                    e.Creatures.SequenceEqual(StoredState.Creatures) &&
                    e.CurrentCreatureIndex == StoredState.CurrentCreatureIndex &&
                    e.CurrentTurn == StoredState.TurnNumber &&
                    e.CurrentMode == StoredState.CurrentMode));
        }
    }
}