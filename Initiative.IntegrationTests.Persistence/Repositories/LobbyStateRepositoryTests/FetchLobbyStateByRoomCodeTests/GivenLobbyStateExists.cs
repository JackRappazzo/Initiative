using System;
using System.Linq;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using Initiative.Persistence.Models.Lobby;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.LobbyStateRepositoryTests.FetchLobbyStateByRoomCodeTests
{
    public class GivenLobbyStateExists : WhenTestingFetchLobbyStateByRoomCode
    {
        protected LobbyCreatureStateDto[] ExpectedCreatures;
        protected int ExpectedTurnNumber;
        protected int ExpectedCurrentCreatureIndex;
        protected LobbyMode ExpectedCurrentMode;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(LobbyStateExists)
            .When(FetchLobbyStateByRoomCodeIsCalled)
            .Then(ShouldReturnLobbyState);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "TEST123";
        }

        [Given]
        public async Task LobbyStateExists()
        {
            ExpectedCreatures = new[]
            {
                new LobbyCreatureStateDto { DisplayName = "Creature1" },
                new LobbyCreatureStateDto { DisplayName = "Creature2" },
                new LobbyCreatureStateDto { DisplayName = "Creature3" }
            };
            ExpectedTurnNumber = 2;
            ExpectedCurrentCreatureIndex = 1;
            ExpectedCurrentMode = LobbyMode.InProgress;

            await LobbyStateRepository.UpsertLobbyState(RoomCode, ExpectedCreatures, ExpectedTurnNumber, ExpectedCurrentCreatureIndex, ExpectedCurrentMode, CancellationToken);
        }

        [Then]
        public void ShouldReturnLobbyState()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.RoomCode, Is.EqualTo(RoomCode));
            Assert.That(Result.Creatures.Select(c => c.DisplayName), Is.EquivalentTo(ExpectedCreatures.Select(c => c.DisplayName)));
            Assert.That(Result.TurnNumber, Is.EqualTo(ExpectedTurnNumber));
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(ExpectedCurrentCreatureIndex));
            Assert.That(Result.CurrentMode, Is.EqualTo(ExpectedCurrentMode));
            Assert.That(Result.Id, Is.Not.Null);
            Assert.That(Result.Id, Is.Not.Empty);
        }
    }
}