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
    public class GivenEmptyCreatureList : WhenTestingUpsertSystemBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryNameIsSet)
            .And(EmptyCreatureListIsSet)
            .When(UpsertSystemBestiaryIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnBestiaryId)
            .And(ShouldCreateBestiaryWithEmptyCreatureList);

        [Given]
        public void BestiaryNameIsSet()
        {
            Name = "Empty Bestiary";
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
        public void ShouldReturnBestiaryId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
        }

        [Then]
        public async Task ShouldCreateBestiaryWithEmptyCreatureList()
        {
            var bestiary = await BeastiaryRepository.GetSystemBestiary(Result, CancellationToken);
            
            Assert.That(bestiary, Is.Not.Null);
            Assert.That(bestiary.Name, Is.EqualTo(Name));
            Assert.That(bestiary.OwnerId, Is.Null);
            Assert.That(bestiary.Creatures, Is.Not.Null);
            Assert.That(bestiary.Creatures, Is.Empty, "Should have empty creature list");
        }
    }
}