using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.GetSystemBestiaryByNameTests
{
    public class GivenBestiaryExistsWithCreatures : WhenTestingGetSystemBestiaryByName
    {
        private string _createdBestiaryId;
        private Creature _testCreature;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryNameIsSet)
            .And(SystemBestiaryExists)
            .And(CreatureIsAddedToBestiary)
            .When(GetSystemBestiaryByNameIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnBestiaryWithCreatures)
            .And(ShouldContainAddedCreature);

        [Given]
        public void BestiaryNameIsSet()
        {
            Name = "Bestiary with Creatures";
        }

        [Given]
        public async Task SystemBestiaryExists()
        {
            _createdBestiaryId = await BeastiaryRepository.CreateSystemBestiary(Name, CancellationToken);
        }

        [Given]
        public async Task CreatureIsAddedToBestiary()
        {
            _testCreature = new Creature
            {
                Name = "Test Dragon",
                SystemName = "test-dragon",
                ArmorClass = 18,
                HitPoints = 200,
                MaximumHitPoints = 200,
                InitiativeModifier = 2,
                WalkSpeed = 40,
                FlySpeed = 80,
                CanHover = false,
                IsPlayer = false,
                IsConcentrating = false
            };

            await BeastiaryRepository.AddCreature(_createdBestiaryId, _testCreature, CancellationToken);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnBestiaryWithCreatures()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Id, Is.EqualTo(_createdBestiaryId));
            Assert.That(Result.Name, Is.EqualTo(Name));
            Assert.That(Result.Creatures, Is.Not.Null);
            Assert.That(Result.Creatures, Is.Not.Empty);
        }

        [Then]
        public void ShouldContainAddedCreature()
        {
            var foundCreature = Result.Creatures.FirstOrDefault(c => c.SystemName == _testCreature.SystemName);
            Assert.That(foundCreature, Is.Not.Null);
            Assert.That(foundCreature.Name, Is.EqualTo(_testCreature.Name));
            Assert.That(foundCreature.ArmorClass, Is.EqualTo(_testCreature.ArmorClass));
            Assert.That(foundCreature.HitPoints, Is.EqualTo(_testCreature.HitPoints));
            Assert.That(foundCreature.WalkSpeed, Is.EqualTo(_testCreature.WalkSpeed));
            Assert.That(foundCreature.FlySpeed, Is.EqualTo(_testCreature.FlySpeed));
        }
    }
}