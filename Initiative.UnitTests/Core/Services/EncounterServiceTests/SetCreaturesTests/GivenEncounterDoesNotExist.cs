using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.SetCreaturesTests
{
    public class GivenEncounterDoesNotExist : WhenTestingSetCreatures
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterIdIsSet)
            .And(OwnerIdIsSet)
            .And(CreaturesAreSet)
            .And(RepositoryDoesNotFindEncounter)
            .When(SetCreaturesIsCalled)
            .Then(ShouldThrowException);

        [Given]
        public void EncounterIdIsSet()
        {
            EncounterId = Guid.NewGuid().ToString();
        }

        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = Guid.NewGuid().ToString();
        }

        [Given]
        public void CreaturesAreSet()
        {
            Creatures = new List<Creature>
            {
                new Creature { Id = Guid.NewGuid().ToString(), Name = "Creature 1" },
                new Creature { Id = Guid.NewGuid().ToString(), Name = "Creature 2" }
            };
        }

        [Given]
        public void RepositoryDoesNotFindEncounter()
        {
            EncounterRepository.FetchEncounterById(EncounterId, OwnerId, CancellationToken)
                .ReturnsNull(); // Simulating that the encounter does not exist
        }

        [Then]
        public void ShouldThrowException()
        {
            Assert.That(ThrownException, Is.TypeOf<ArgumentException>());
        }
    }
}
