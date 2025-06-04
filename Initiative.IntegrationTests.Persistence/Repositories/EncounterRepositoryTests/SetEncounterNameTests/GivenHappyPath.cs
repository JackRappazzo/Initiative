using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.SetEncounterNameTests
{
    public class GivenHappyPath : WhenTestingSetEncounterName
    {
        string OwnerId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterExists)
            .And(NewNameIsSet)
            .When(SetEncounterNameIsCalled)
            .Then(ShouldUpdateEncounterName);

        [Given]
        public async Task EncounterExists()
        {
            OwnerId = ObjectId.GenerateNewId().ToString();
            string displayName = "TestEncounter";
            EncounterId = await EncounterRepository.CreateEncounter(OwnerId, displayName, CancellationToken);
        }

        [Then]
        public async Task ShouldUpdateEncounterName()
        {
            var encounter = await EncounterRepository.FetchEncounterById(EncounterId, OwnerId, CancellationToken);
            Assert.That(encounter, Is.Not.Null, "Encounter should not be null.");
            Assert.That(encounter.DisplayName, Is.EqualTo(NewName), "Encounter name should be updated to the new name.");
        }
    }
}
