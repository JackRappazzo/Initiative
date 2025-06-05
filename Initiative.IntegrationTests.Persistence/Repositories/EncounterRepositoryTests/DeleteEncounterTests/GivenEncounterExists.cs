using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.DeleteEncounterTests
{
    public class GivenEncounterExists : WhenTestingDeleteEncounter
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterExists)
            .When(DeleteEncounterIsCalled)
            .Then(ShouldReturnTrue);

        [Given]
        public async Task EncounterExists()
        {
            EncounterId = await EncounterRepository.CreateEncounter(ObjectId.GenerateNewId().ToString(), "Test Encounter", CancellationToken);
        }

        [Then]
        public void ShouldReturnTrue()
        {
            Assert.That(Result, Is.True, "Encounter deletion should return true when the encounter exists.");
        }
    }
}
