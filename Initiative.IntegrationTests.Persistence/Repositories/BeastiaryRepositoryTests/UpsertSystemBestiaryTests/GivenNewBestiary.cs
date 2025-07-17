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
    public class GivenNewBestiary : WhenTestingUpsertSystemBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryNameIsSet)
            .And(CreaturesAreSet)
            .When(UpsertSystemBestiaryIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnBestiaryId)
            .And(ShouldCreateNewBestiary)
            .And(ShouldStoreAllCreatures);

        [Given]
        public void BestiaryNameIsSet()
        {
            Name = "New Monster Manual";
        }

        [Given]
        public void CreaturesAreSet()
        {
            Creatures = new List<Creature>
            {
                new Creature
                {
                    Name = "Adult Red Dragon",
                    SystemName = "adult-red-dragon",
                    ArmorClass = 19,
                    HitPoints = 256,
                    MaximumHitPoints = 256,
                    InitiativeModifier = 0,
                    WalkSpeed = 40,
                    FlySpeed = 80,
                    CanHover = false,
                    IsPlayer = false,
                    IsConcentrating = false
                },
                new Creature
                {
                    Name = "Goblin",
                    SystemName = "goblin",
                    ArmorClass = 15,
                    HitPoints = 7,
                    MaximumHitPoints = 7,
                    InitiativeModifier = 2,
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
        public void ShouldReturnBestiaryId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
        }

        [Then]
        public async Task ShouldCreateNewBestiary()
        {
            var bestiary = await BeastiaryRepository.GetSystemBestiary(Result, CancellationToken);
            
            Assert.That(bestiary, Is.Not.Null);
            Assert.That(bestiary.Id, Is.EqualTo(Result));
            Assert.That(bestiary.Name, Is.EqualTo(Name));
            Assert.That(bestiary.OwnerId, Is.Null); // System bestiary should have null OwnerId
        }

        [Then]
        public async Task ShouldStoreAllCreatures()
        {
            var bestiary = await BeastiaryRepository.GetSystemBestiary(Result, CancellationToken);
            
            Assert.That(bestiary.Creatures, Is.Not.Null);
            Assert.That(bestiary.Creatures.Count(), Is.EqualTo(2));
            
            var dragon = bestiary.Creatures.FirstOrDefault(c => c.SystemName == "adult-red-dragon");
            Assert.That(dragon, Is.Not.Null);
            Assert.That(dragon.Name, Is.EqualTo("Adult Red Dragon"));
            Assert.That(dragon.ArmorClass, Is.EqualTo(19));
            Assert.That(dragon.FlySpeed, Is.EqualTo(80));
            
            var goblin = bestiary.Creatures.FirstOrDefault(c => c.SystemName == "goblin");
            Assert.That(goblin, Is.Not.Null);
            Assert.That(goblin.Name, Is.EqualTo("Goblin"));
            Assert.That(goblin.ArmorClass, Is.EqualTo(15));
        }
    }
}