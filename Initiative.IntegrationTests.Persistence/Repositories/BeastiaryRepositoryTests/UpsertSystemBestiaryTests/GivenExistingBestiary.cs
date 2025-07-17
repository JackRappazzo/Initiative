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
    public class GivenExistingBestiary : WhenTestingUpsertSystemBestiary
    {
        private string _existingBestiaryId;
        private List<Creature> _originalCreatures;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryNameIsSet)
            .And(ExistingBestiaryExists)
            .And(NewCreaturesAreSet)
            .When(UpsertSystemBestiaryIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnSameBestiaryId)
            .And(ShouldUpdateExistingBestiary)
            .And(ShouldReplaceAllCreatures);

        [Given]
        public void BestiaryNameIsSet()
        {
            Name = "Existing Monster Manual";
        }

        [Given]
        public async Task ExistingBestiaryExists()
        {
            // Create an existing bestiary with some creatures
            _originalCreatures = new List<Creature>
            {
                new Creature
                {
                    Name = "Original Dragon",
                    SystemName = "original-dragon",
                    ArmorClass = 15,
                    HitPoints = 100,
                    MaximumHitPoints = 100,
                    InitiativeModifier = 1,
                    IsPlayer = false,
                    IsConcentrating = false
                }
            };

            _existingBestiaryId = await BeastiaryRepository.CreateSystemBestiary(Name, CancellationToken);
            
            // Add the original creatures
            foreach (var creature in _originalCreatures)
            {
                await BeastiaryRepository.AddCreature(_existingBestiaryId, creature, CancellationToken);
            }
        }

        [Given]
        public void NewCreaturesAreSet()
        {
            Creatures = new List<Creature>
            {
                new Creature
                {
                    Name = "Updated Red Dragon",
                    SystemName = "updated-red-dragon",
                    ArmorClass = 20,
                    HitPoints = 300,
                    MaximumHitPoints = 300,
                    InitiativeModifier = 2,
                    WalkSpeed = 40,
                    FlySpeed = 80,
                    IsPlayer = false,
                    IsConcentrating = false
                },
                new Creature
                {
                    Name = "New Orc",
                    SystemName = "new-orc",
                    ArmorClass = 13,
                    HitPoints = 15,
                    MaximumHitPoints = 15,
                    InitiativeModifier = 1,
                    WalkSpeed = 30,
                    IsPlayer = false,
                    IsConcentrating = false
                },
                new Creature
                {
                    Name = "Updated Goblin",
                    SystemName = "updated-goblin",
                    ArmorClass = 16,
                    HitPoints = 10,
                    MaximumHitPoints = 10,
                    InitiativeModifier = 3,
                    WalkSpeed = 30,
                    IsPlayer = false,
                    IsConcentrating = false
                }
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnSameBestiaryId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.EqualTo(_existingBestiaryId), "Should return the same ID when updating existing bestiary");
        }

        [Then]
        public async Task ShouldUpdateExistingBestiary()
        {
            var bestiary = await BeastiaryRepository.GetSystemBestiary(Result, CancellationToken);
            
            Assert.That(bestiary, Is.Not.Null);
            Assert.That(bestiary.Id, Is.EqualTo(_existingBestiaryId));
            Assert.That(bestiary.Name, Is.EqualTo(Name));
            Assert.That(bestiary.OwnerId, Is.Null);
        }

        [Then]
        public async Task ShouldReplaceAllCreatures()
        {
            var bestiary = await BeastiaryRepository.GetSystemBestiary(Result, CancellationToken);
            
            Assert.That(bestiary.Creatures, Is.Not.Null);
            Assert.That(bestiary.Creatures.Count(), Is.EqualTo(3), "Should have all new creatures");
            
            // Verify original creatures are gone
            var originalCreature = bestiary.Creatures.FirstOrDefault(c => c.SystemName == "original-dragon");
            Assert.That(originalCreature, Is.Null, "Original creatures should be replaced");
            
            // Verify new creatures are present
            var updatedDragon = bestiary.Creatures.FirstOrDefault(c => c.SystemName == "updated-red-dragon");
            Assert.That(updatedDragon, Is.Not.Null);
            Assert.That(updatedDragon.ArmorClass, Is.EqualTo(20));
            
            var newOrc = bestiary.Creatures.FirstOrDefault(c => c.SystemName == "new-orc");
            Assert.That(newOrc, Is.Not.Null);
            Assert.That(newOrc.Name, Is.EqualTo("New Orc"));
            
            var updatedGoblin = bestiary.Creatures.FirstOrDefault(c => c.SystemName == "updated-goblin");
            Assert.That(updatedGoblin, Is.Not.Null);
            Assert.That(updatedGoblin.InitiativeModifier, Is.EqualTo(3));
        }
    }
}