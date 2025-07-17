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
    public class GivenExistingBestiaryWithCreaturesAndEmptyUpdate : WhenTestingUpsertSystemBestiary
    {
        private string _existingBestiaryId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryNameIsSet)
            .And(ExistingBestiaryWithCreaturesExists)
            .And(EmptyCreatureListIsSet)
            .When(UpsertSystemBestiaryIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnSameBestiaryId)
            .And(ShouldClearAllCreatures);

        [Given]
        public void BestiaryNameIsSet()
        {
            Name = "Bestiary to Clear";
        }

        [Given]
        public async Task ExistingBestiaryWithCreaturesExists()
        {
            _existingBestiaryId = await BeastiaryRepository.CreateSystemBestiary(Name, CancellationToken);
            
            // Add some creatures to the existing bestiary
            var originalCreatures = new List<Creature>
            {
                new Creature
                {
                    Name = "Dragon to Remove",
                    SystemName = "dragon-to-remove",
                    ArmorClass = 18,
                    HitPoints = 150,
                    MaximumHitPoints = 150,
                    IsPlayer = false,
                    IsConcentrating = false
                },
                new Creature
                {
                    Name = "Orc to Remove",
                    SystemName = "orc-to-remove",
                    ArmorClass = 13,
                    HitPoints = 15,
                    MaximumHitPoints = 15,
                    IsPlayer = false,
                    IsConcentrating = false
                }
            };

            foreach (var creature in originalCreatures)
            {
                await BeastiaryRepository.AddCreature(_existingBestiaryId, creature, CancellationToken);
            }
        }

        [Given]
        public void EmptyCreatureListIsSet()
        {
            Creatures = new List<Creature>();
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
        public async Task ShouldClearAllCreatures()
        {
            var bestiary = await BeastiaryRepository.GetSystemBestiary(Result, CancellationToken);
            
            Assert.That(bestiary, Is.Not.Null);
            Assert.That(bestiary.Name, Is.EqualTo(Name));
            Assert.That(bestiary.Creatures, Is.Not.Null);
            Assert.That(bestiary.Creatures, Is.Empty, "All creatures should be removed when updating with empty list");
        }
    }
}