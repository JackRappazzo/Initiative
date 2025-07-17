using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.UpsertSystemBestiaryTests
{
    public class GivenLargeCreatureList : WhenTestingUpsertSystemBestiary
    {
        private List<Creature> _largeCreatureList;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryNameIsSet)
            .And(LargeCreatureListIsSet)
            .When(UpsertSystemBestiaryIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnBestiaryId)
            .And(ShouldStoreAllCreatures);

        [Given]
        public void BestiaryNameIsSet()
        {
            Name = "Large Bestiary";
        }

        [Given]
        public void LargeCreatureListIsSet()
        {
            _largeCreatureList = new List<Creature>();
            
            // Create a large list of creatures to test performance and batch operations
            for (int i = 1; i <= 100; i++)
            {
                _largeCreatureList.Add(new Creature
                {
                    Name = $"Test Creature {i}",
                    SystemName = $"test-creature-{i}",
                    ArmorClass = 10 + (i % 10),
                    HitPoints = 5 + (i % 20),
                    MaximumHitPoints = 5 + (i % 20),
                    InitiativeModifier = i % 5,
                    WalkSpeed = 30,
                    IsPlayer = false,
                    IsConcentrating = false
                });
            }
            
            Creatures = _largeCreatureList;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnBestiaryId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
        }

        [Then]
        public async Task ShouldStoreAllCreatures()
        {
            var bestiary = await BeastiaryRepository.GetSystemBestiary(Result, CancellationToken);
            
            Assert.That(bestiary, Is.Not.Null);
            Assert.That(bestiary.Name, Is.EqualTo(Name));
            Assert.That(bestiary.Creatures, Is.Not.Null);
            Assert.That(bestiary.Creatures.Count(), Is.EqualTo(100), "Should store all 100 creatures");
            
            // Verify some specific creatures exist
            var firstCreature = bestiary.Creatures.FirstOrDefault(c => c.SystemName == "test-creature-1");
            Assert.That(firstCreature, Is.Not.Null);
            Assert.That(firstCreature.Name, Is.EqualTo("Test Creature 1"));
            
            var lastCreature = bestiary.Creatures.FirstOrDefault(c => c.SystemName == "test-creature-100");
            Assert.That(lastCreature, Is.Not.Null);
            Assert.That(lastCreature.Name, Is.EqualTo("Test Creature 100"));
            
            var middleCreature = bestiary.Creatures.FirstOrDefault(c => c.SystemName == "test-creature-50");
            Assert.That(middleCreature, Is.Not.Null);
            Assert.That(middleCreature.Name, Is.EqualTo("Test Creature 50"));
        }
    }
}