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

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.UpsertSystemBestiaryTests
{
    public class GivenUserBestiaryExistsWithSameName : WhenTestingUpsertSystemBestiary
    {
        private string _userId;
        private string _userBestiaryId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryNameIsSet)
            .And(UserIdIsSet)
            .And(UserBestiaryExistsWithSameName)
            .And(CreaturesAreSet)
            .When(UpsertSystemBestiaryIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldCreateNewSystemBestiary)
            .And(ShouldNotAffectUserBestiary);

        [Given]
        public void BestiaryNameIsSet()
        {
            Name = "Shared Name Bestiary";
        }

        [Given]
        public void UserIdIsSet()
        {
            _userId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public async Task UserBestiaryExistsWithSameName()
        {
            _userBestiaryId = await BeastiaryRepository.CreateUserBestiary(_userId, Name, CancellationToken);
            
            // Add a creature to the user bestiary
            var userCreature = new Creature
            {
                Name = "User's Dragon",
                SystemName = "users-dragon",
                ArmorClass = 17,
                HitPoints = 120,
                MaximumHitPoints = 120,
                IsPlayer = false,
                IsConcentrating = false
            };
            
            await BeastiaryRepository.AddCreature(_userBestiaryId, userCreature, CancellationToken);
        }

        [Given]
        public void CreaturesAreSet()
        {
            Creatures = new List<Creature>
            {
                new Creature
                {
                    Name = "System Dragon",
                    SystemName = "system-dragon",
                    ArmorClass = 20,
                    HitPoints = 300,
                    MaximumHitPoints = 300,
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
        public async Task ShouldCreateNewSystemBestiary()
        {
            var systemBestiary = await BeastiaryRepository.GetSystemBestiary(Result, CancellationToken);
            
            Assert.That(systemBestiary, Is.Not.Null);
            Assert.That(systemBestiary.Id, Is.Not.EqualTo(_userBestiaryId), "Should create a separate system bestiary");
            Assert.That(systemBestiary.Name, Is.EqualTo(Name));
            Assert.That(systemBestiary.OwnerId, Is.Null, "System bestiary should have null OwnerId");
            Assert.That(systemBestiary.Creatures.Count(), Is.EqualTo(1));
            
            var systemDragon = systemBestiary.Creatures.FirstOrDefault(c => c.SystemName == "system-dragon");
            Assert.That(systemDragon, Is.Not.Null);
        }

        [Then]
        public async Task ShouldNotAffectUserBestiary()
        {
            var userBestiary = await BeastiaryRepository.GetUserBestiary(_userBestiaryId, _userId, CancellationToken);
            
            Assert.That(userBestiary, Is.Not.Null);
            Assert.That(userBestiary.Id, Is.EqualTo(_userBestiaryId));
            Assert.That(userBestiary.OwnerId, Is.EqualTo(_userId));
            Assert.That(userBestiary.Creatures.Count(), Is.EqualTo(1));
            
            var userDragon = userBestiary.Creatures.FirstOrDefault(c => c.SystemName == "users-dragon");
            Assert.That(userDragon, Is.Not.Null, "User's original creature should remain unchanged");
        }
    }
}