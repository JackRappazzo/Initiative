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

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.SetCreaturesTests
{
    public class GivenHappyPath : WhenTestingSetCreatures
    {

        protected string OwnerId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterExists)
            .And(CreaturesAreSet)
            .When(SetCreaturesIsCalled)
            .Then(ShouldStoreCreatures);


        [Given]
        public async Task EncounterExists()
        {
            OwnerId = ObjectId.GenerateNewId().ToString();
            string displayName = "TestEncounter";
            EncounterId = await EncounterRepository.CreateEncounter(OwnerId, displayName, CancellationToken);
        }

        [Given]
        public void CreaturesAreSet()
        {
            EncounterCreature creatureOne = new EncounterCreature()
            {
                DisplayName = "Test Creature One",
                CreatureName = "test-creature-one",
                AC = 10,
                CurrentHP = 20,
                MaxHP = 10,
                Initiative = 15,
                IsPlayer = false
            };

            EncounterCreature creatureTwo = new EncounterCreature()
            {
                DisplayName = "Test Creature Two",
                CreatureName = "test-creature-two",
                AC = 12,
                CurrentHP = 25,
                MaxHP = 5,
                Initiative = 18,
                IsPlayer = false
            };

            Creatures = new List<EncounterCreature> { creatureOne, creatureTwo };
        }

        [Then]
        public async Task ShouldStoreCreatures()
        {
            var encounter = await EncounterRepository.FetchEncounterById(EncounterId, OwnerId, CancellationToken);
            Assert.That(encounter.Creatures, Is.Not.Null);
            Assert.That(encounter.Creatures.Count(), Is.EqualTo(Creatures.Count()));
            Assert.That(encounter.Creatures.Any(c => c.DisplayName == "Test Creature One"), Is.True);
            Assert.That(encounter.Creatures.Any(c => c.DisplayName == "Test Creature Two"), Is.True);

            var creatureOne = encounter.Creatures.First(c => c.DisplayName == "Test Creature One");

            Assert.That(creatureOne.MaxHP, Is.EqualTo(10));
            Assert.That(creatureOne.AC, Is.EqualTo(10));
            Assert.That(creatureOne.Initiative, Is.EqualTo(15));
            Assert.That(creatureOne.IsPlayer, Is.False);
        }
    }
}
