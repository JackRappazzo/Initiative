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
            Creature creatureOne = new Creature()
            {
                DisplayName = "Test Creature One",
                SystemName = "test-creature-one",
                ArmorClass = 10,
                HitPoints = 20,
                Initiative = 15,
                InitiativeModifier = 2,
                IsConcentrating = false,
                IsPlayer = false
            };

            Creature creatureTwo = new Creature()
            {
                DisplayName = "Test Creature Two",
                SystemName = "test-creature-two",
                ArmorClass = 12,
                HitPoints = 25,
                Initiative = 18,
                InitiativeModifier = 3,
                IsConcentrating = true,
                IsPlayer = false
            };

            Creatures = new List<Creature> { creatureOne, creatureTwo };
        }

        [Then]
        public async Task ShouldStoreCreatures()
        {
            var encounter = await EncounterRepository.FetchEncounterById(EncounterId, OwnerId, CancellationToken);
            Assert.That(encounter.Creatures, Is.Not.Null);
            Assert.That(encounter.Creatures.Count(), Is.EqualTo(Creatures.Count()));
            Assert.That(encounter.Creatures.Any(c => c.DisplayName == "Test Creature One"), Is.True);
            Assert.That(encounter.Creatures.Any(c => c.DisplayName == "Test Creature Two"), Is.True);
        }
    }
}
