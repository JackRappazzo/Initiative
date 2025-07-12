using System;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.LobbyStateRepositoryTests.UpsertLobbyStateTests
{
    public class GivenNewLobbyState : WhenTestingUpsertLobbyState
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(CreaturesAreSet)
            .And(TurnNumberIsSet)
            .And(CurrentCreatureIndexIsSet)
            .And(CurrentModeIsSet)
            .When(UpsertLobbyStateIsCalled)
            .Then(ShouldReturnId)
            .And(ShouldStoreLobbyState);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "NEWSTT";
        }

        [Given]
        public void CreaturesAreSet()
        {
            Creatures = new[] { "Creature1", "Creature2", "Creature3" };
        }

        [Given]
        public void TurnNumberIsSet()
        {
            TurnNumber = 1;
        }

        [Given]
        public void CurrentCreatureIndexIsSet()
        {
            CurrentCreatureIndex = 0;
        }

        [Given]
        public void CurrentModeIsSet()
        {
            CurrentMode = LobbyMode.InProgress;
        }

        [Then]
        public void ShouldReturnId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
        }

        [Then]
        public async Task ShouldStoreLobbyState()
        {
            var storedState = await LobbyStateRepository.FetchLobbyStateByRoomCode(RoomCode, CancellationToken);

            Assert.That(storedState, Is.Not.Null);
            Assert.That(storedState.Id, Is.EqualTo(Result));
            Assert.That(storedState.RoomCode, Is.EqualTo(RoomCode));
            Assert.That(storedState.Creatures, Is.EquivalentTo(Creatures));
            Assert.That(storedState.TurnNumber, Is.EqualTo(TurnNumber));
            Assert.That(storedState.CurrentCreatureIndex, Is.EqualTo(CurrentCreatureIndex));
            Assert.That(storedState.CurrentMode, Is.EqualTo(CurrentMode));
        }
    }
}