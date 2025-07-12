using System;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.LobbyStateRepositoryTests.UpsertLobbyStateTests
{
    public class GivenExistingLobbyState : WhenTestingUpsertLobbyState
    {
        protected string[] OriginalCreatures;
        protected int OriginalTurnNumber;
        protected int OriginalCurrentCreatureIndex;
        protected LobbyMode OriginalCurrentMode;
        protected string OriginalId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(OriginalLobbyStateExists)
            .And(UpdatedCreaturesAreSet)
            .And(UpdatedTurnNumberIsSet)
            .And(UpdatedCurrentCreatureIndexIsSet)
            .And(UpdatedCurrentModeIsSet)
            .When(UpsertLobbyStateIsCalled)
            .Then(ShouldReturnId)
            .And(ShouldUpdateLobbyState);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "TEST123";
        }

        [Given]
        public async Task OriginalLobbyStateExists()
        {
            OriginalCreatures = new[] { "OriginalCreature1", "OriginalCreature2" };
            OriginalTurnNumber = 1;
            OriginalCurrentCreatureIndex = 0;
            OriginalCurrentMode = LobbyMode.Waiting;

            OriginalId = await LobbyStateRepository.UpsertLobbyState(RoomCode, OriginalCreatures, OriginalTurnNumber, OriginalCurrentCreatureIndex, OriginalCurrentMode, CancellationToken);
        }

        [Given]
        public void UpdatedCreaturesAreSet()
        {
            Creatures = new[] { "UpdatedCreature1", "UpdatedCreature2", "UpdatedCreature3" };
        }

        [Given]
        public void UpdatedTurnNumberIsSet()
        {
            TurnNumber = 3;
        }

        [Given]
        public void UpdatedCurrentCreatureIndexIsSet()
        {
            CurrentCreatureIndex = 2;
        }

        [Given]
        public void UpdatedCurrentModeIsSet()
        {
            CurrentMode = LobbyMode.InProgress;
        }

        [Then]
        public void ShouldReturnId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
            Assert.That(Result, Is.EqualTo(OriginalId), "Should return the same ID when updating existing state");
        }

        [Then]
        public async Task ShouldUpdateLobbyState()
        {
            var storedState = await LobbyStateRepository.FetchLobbyStateByRoomCode(RoomCode, CancellationToken);

            Assert.That(storedState, Is.Not.Null);
            Assert.That(storedState.Id, Is.EqualTo(Result));
            Assert.That(storedState.Id, Is.EqualTo(OriginalId), "ID should remain the same when updating");
            Assert.That(storedState.RoomCode, Is.EqualTo(RoomCode));
            Assert.That(storedState.Creatures, Is.EquivalentTo(Creatures));
            Assert.That(storedState.TurnNumber, Is.EqualTo(TurnNumber));
            Assert.That(storedState.CurrentCreatureIndex, Is.EqualTo(CurrentCreatureIndex));
            Assert.That(storedState.CurrentMode, Is.EqualTo(CurrentMode));
            
            // Verify it was updated, not just inserted
            Assert.That(storedState.Creatures, Is.Not.EquivalentTo(OriginalCreatures));
            Assert.That(storedState.TurnNumber, Is.Not.EqualTo(OriginalTurnNumber));
            Assert.That(storedState.CurrentCreatureIndex, Is.Not.EqualTo(OriginalCurrentCreatureIndex));
            Assert.That(storedState.CurrentMode, Is.Not.EqualTo(OriginalCurrentMode));
        }
    }
}