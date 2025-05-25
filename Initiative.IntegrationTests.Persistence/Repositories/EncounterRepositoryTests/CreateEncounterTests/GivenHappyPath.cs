using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.CreateEncounterTests
{
    public class GivenHappyPath : WhenTestingCreateEncounter
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(OwnerIdIsSet)
            .And(NameIsSet)
            .When(CreateEncounterIsCalled)
            .Then(ShouldReturnId)
            .And(ShouldStoreEncounter);

        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public void NameIsSet()
        {
            Name = "Test Encounter";
        }

        [Then]
        public void ShouldReturnId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
        }

        [Then]
        public async Task ShouldStoreEncounter()
        {
            var encounter = await EncounterRepository.FetchEncounterById(Result, OwnerId, CancellationToken);

            Assert.That(encounter, Is.Not.Null);
            Assert.That(encounter.DisplayName, Is.Not.Null);
        }

    }
}
