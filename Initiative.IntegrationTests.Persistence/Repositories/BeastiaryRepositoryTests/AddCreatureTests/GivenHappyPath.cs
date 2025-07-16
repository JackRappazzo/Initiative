using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.AddCreatureTests
{
    public class GivenHappyPath : WhenTestingAddCreature
    {

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(CreatureIsSet)
            .And(BeastiaryExists)
            .When(AddCreatureIsCalled)
            .Then(ShouldReturnCreatureId)
            .And(ShouldStoreCreature);

        [Given]
        public void CreatureIsSet()
        {
            Creature = new Creature()
            {
                ArmorClass = 10,
                Name = "Test Creature",
                HitPoints = 10,
                SystemName = "test-creature"
            };
        }

        [Given]
        public async Task BeastiaryExists()
        {
            BeastiaryId = await BeastiaryRepository.CreateSystemBestiary("test_" + Guid.NewGuid(), CancellationToken);
        }

        [Then]
        public void ShouldReturnCreatureId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
        }

        [Then]
        public async Task ShouldStoreCreature()
        {
            var beastiary = await BeastiaryRepository.GetSystemBestiary(BeastiaryId, CancellationToken);
            Assert.That(beastiary.Creatures, Is.Not.Empty);
            Assert.That(beastiary.Creatures.First().SystemName, Is.EqualTo(Creature.SystemName));
        }
    }
}
