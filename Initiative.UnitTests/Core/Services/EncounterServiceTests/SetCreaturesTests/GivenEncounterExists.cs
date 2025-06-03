using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.SetCreaturesTests
{
    public class GivenEncounterExists : WhenTestingSetCreatures
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterIdIsSet)
            .And(OwnerIdIsSet)
            .And(CreaturesAreSet)
            .And(RepositoryReturnsEncounter)
            .When(SetCreaturesIsCalled)
            .Then(ShouldNotThrowException);

        [Given]
        public void EncounterIdIsSet()
        {
            EncounterId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public void CreaturesAreSet()
        {
            Creatures = new List<Creature>
            {
                new Creature { Id = ObjectId.GenerateNewId().ToString(), Name = "Creature 1" },
                new Creature { Id = ObjectId.GenerateNewId().ToString(), Name = "Creature 2" }
            };
        }

        [Given]
        public void RepositoryReturnsEncounter()
        {
            EncounterRepository.FetchEncounterById(EncounterId, OwnerId, CancellationToken)
                .Returns(new Encounter
                {
                    Id = EncounterId,
                    OwnerId = OwnerId,
                    DisplayName = "Test Encounter " + Guid.NewGuid(),
                    Creatures = new List<Creature>()
                });
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }
    }
}
