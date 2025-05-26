using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.FetchEncounterListTests
{
    public class GivenEncountersToFetch : WhenTestingFetchEncounterList
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .And(UserHasEncounters)
            .And(EncountersHaveNoCreatuers)
            .When(FetchEncounterListIsCalled)
            .Then(ShouldReturnAllEncounters)
            .And(ShouldReturnCorrectData);

        [Given]
        public void UserIdIsSet()
        {
            UserId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public async Task UserHasEncounters()
        {
            await EncounterRepository.CreateEncounter(UserId, "Test Encounter 1", CancellationToken);
            await EncounterRepository.CreateEncounter(UserId, "Test Encounter 2", CancellationToken);
        }

        [Given]
        public async Task EncountersHaveNoCreatuers()
        {
            //Do nothing
        }

        [Then]
        public void ShouldReturnAllEncounters()
        {
            Assert.That(Results, Is.Not.Null);
            Assert.That(Results.Count(), Is.EqualTo(2));
        }

        [Then]
        public void ShouldReturnCorrectData()
        {
            Assert.That(Results.Any(r => r.EncounterName == "Test Encounter 1"), Is.True);
            Assert.That(Results.Any(r => r.EncounterName == "Test Encounter 2"), Is.True);
            Assert.That(Results.All(r => r.NumberOfCreatures == 0), Is.True);
        }
    }
}
