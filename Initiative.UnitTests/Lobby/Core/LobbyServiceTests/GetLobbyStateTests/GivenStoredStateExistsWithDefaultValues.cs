using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using Initiative.Persistence.Models.Lobby;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetLobbyStateTests
{
    public class GivenStoredStateExistsWithDefaultValues : WhenTestingGetLobbyState
    {
        protected LobbyStateDto StoredState;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(StoredStateWithDefaultValuesExists)
            .When(GetLobbyStateIsCalled)
            .Then(ShouldReturnDefaultState)
            .And(ShouldCallSetLobbyState);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "DEFAULT123";
        }

        [Given]
        public void StoredStateWithDefaultValuesExists()
        {
            StoredState = new LobbyStateDto
            {
                Id = "test-id",
                RoomCode = RoomCode,
                Creatures = new string[0],
                TurnNumber = 0,
                CurrentCreatureIndex = 0,
                CurrentMode = LobbyMode.Waiting
            };

            LobbyStateRepository.FetchLobbyStateByRoomCode(RoomCode, CancellationToken)
                .Returns(Task.FromResult(StoredState));
        }

        [Then]
        public void ShouldReturnDefaultState()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Creatures, Is.Empty);
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(0));
            Assert.That(Result.CurrentTurn, Is.EqualTo(0));
            Assert.That(Result.CurrentMode, Is.EqualTo(LobbyMode.Waiting));
        }

        [Then]
        public async Task ShouldCallSetLobbyState()
        {
            // Verify that SetLobbyState was called to update the in-memory state
            await LobbyStateRepository.Received(1).UpsertLobbyState(
                RoomCode,
                Arg.Is<string[]>(creatures => creatures.Length == 0),
                0,
                0,
                LobbyMode.Waiting,
                CancellationToken);
        }
    }
}