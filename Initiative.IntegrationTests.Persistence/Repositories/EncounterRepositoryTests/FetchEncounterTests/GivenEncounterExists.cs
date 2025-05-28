using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.FetchEncounterTests
{
    public class GivenEncounterExists : WhenTestingFetchEncounter
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .And(EncounterExists)
            .When(FetchEncounterIsCalled)
            .Then(ShouldReturnEncounter);


        [Given]
        public async Task EncounterExists()
        {
            EncounterId = await EncounterRepository.CreateEncounter(UserId, "Test Encounter", CancellationToken);
        }


        [Then]
        public void ShouldReturnEncounter()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(EncounterId, Is.EqualTo(Result.Id));
            Assert.That(UserId, Is.EqualTo(Result.OwnerId));
            Assert.That(Result.DisplayName, Is.EqualTo("Test Encounter"));
        }
    }
}
