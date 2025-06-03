using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.UnitTests.Core.Services.EncounterServiceTests.GetEncounterTests;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.GetEncounterTests
{
    public class GivenEncounterExists : WhenTestingGetEncounter
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(OwnerIdIsSet)
            .And(EncounterIdIsSet)
            .And(RepositoryReturnsEncounter)
            .When(FetchEncounterIsCalled)
            .Then(ShouldReturnEncounter)
            .And(ShouldNotBeNull);

        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public void EncounterIdIsSet()
        {
            EncounterId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public void RepositoryReturnsEncounter()
        {
            EncounterRepository.FetchEncounterById(EncounterId, OwnerId, CancellationToken)
                .Returns(new Initiative.Persistence.Models.Encounters.Encounter
                {
                    Id = EncounterId,
                    OwnerId = OwnerId,
                    DisplayName = "Test Encounter " + Guid.NewGuid()
                });
        }

        [Then]
        public void ShouldReturnEncounter()
        {
            Assert.That(Result, Is.Not.Null, "Result should not be null.");
            Assert.That(Result.Id, Is.EqualTo(EncounterId), "Encounter ID should match the requested ID.");
            Assert.That(Result.OwnerId, Is.EqualTo(OwnerId), "Owner ID should match the requested Owner ID.");
            Assert.That(Result.DisplayName, Is.Not.Null.Or.Empty, "Encounter Display Name should not be null or empty.");
        }
    }
}
