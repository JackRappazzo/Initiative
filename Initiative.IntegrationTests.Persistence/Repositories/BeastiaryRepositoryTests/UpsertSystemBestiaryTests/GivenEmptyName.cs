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
    public class GivenEmptyName : WhenTestingUpsertSystemBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EmptyNameIsSet)
            .And(CreaturesAreSet)
            .When(UpsertSystemBestiaryIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldCreateBestiaryWithEmptyName);

        [Given]
        public void EmptyNameIsSet()
        {
            Name = string.Empty;
        }

        [Given]
        public void CreaturesAreSet()
        {
            Creatures = new List<Creature>
            {
                new Creature
                {
                    Name = "Test Creature",
                    SystemName = "test-creature",
                    ArmorClass = 15,
                    HitPoints = 30,
                    MaximumHitPoints = 30,
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
        public async Task ShouldCreateBestiaryWithEmptyName()
        {
            var bestiary = await BeastiaryRepository.GetSystemBestiary(Result, CancellationToken);
            
            Assert.That(bestiary, Is.Not.Null);
            Assert.That(bestiary.Name, Is.EqualTo(string.Empty));
            Assert.That(bestiary.OwnerId, Is.Null);
            Assert.That(bestiary.Creatures.Count(), Is.EqualTo(1));
        }
    }
}