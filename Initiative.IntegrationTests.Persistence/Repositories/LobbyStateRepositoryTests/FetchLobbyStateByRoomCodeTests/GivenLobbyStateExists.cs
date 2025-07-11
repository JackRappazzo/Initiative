using System;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.LobbyStateRepositoryTests.FetchLobbyStateByRoomCodeTests
{
    public class GivenLobbyStateExists : WhenTestingFetchLobbyStateByRoomCode
    {
        protected string[] ExpectedCreatures;
        protected int ExpectedTurnNumber;
        protected int ExpectedCurrentCreatureIndex;

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
            ExpectedCreatures = new[] { "Creature1", "Creature2", "Creature3" };
            ExpectedTurnNumber = 2;
            ExpectedCurrentCreatureIndex = 1;

            await LobbyStateRepository.UpsertLobbyState(RoomCode, ExpectedCreatures, ExpectedTurnNumber, ExpectedCurrentCreatureIndex, CancellationToken);
        }

        [Then]
        public void ShouldReturnLobbyState()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.RoomCode, Is.EqualTo(RoomCode));
            Assert.That(Result.Creatures, Is.EquivalentTo(ExpectedCreatures));
            Assert.That(Result.TurnNumber, Is.EqualTo(ExpectedTurnNumber));
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(ExpectedCurrentCreatureIndex));
            Assert.That(Result.Id, Is.Not.Null);
            Assert.That(Result.Id, Is.Not.Empty);
        }
    }
}