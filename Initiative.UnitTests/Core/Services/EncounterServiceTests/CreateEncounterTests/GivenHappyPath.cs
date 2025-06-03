using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.CreateEncounterTests
{
    public class GivenHappyPath : WhenTestingCreateEncounter
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(OwnerIdIsSet)
            .And(EncounterNameIsSet)
            .And(EncounterRepositoryReturnsNewId)
            .When(CreateEncounterIsCalled)
            .Then(ResultShouldNotBeNullOrEmpty)
            .And(ShouldReturnEncounterId);

        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = Guid.NewGuid().ToString();
        }

        [Given]
        public void EncounterNameIsSet()
        {
            EncounterName = "Test Encounter " + Guid.NewGuid();
        }

        [Given]
        public void EncounterRepositoryReturnsNewId()
        {
            EncounterRepository.CreateEncounter(OwnerId, EncounterName, CancellationToken)
                .Returns(ObjectId.GenerateNewId().ToString());
        }

        [Then]
        public void ShouldReturnEncounterId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty, "Encounter ID should not be null or empty.");

        }
    }
}
