using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.GetSystemBestiaryByNameTests
{
    public class GivenUserBestiaryExistsWithSameName : WhenTestingGetSystemBestiaryByName
    {
        private string _userId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryNameIsSet)
            .And(UserIdIsSet)
            .And(UserBestiaryExistsWithSameName)
            .When(GetSystemBestiaryByNameIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnNullDespiteUserBestiaryExisting);

        [Given]
        public void BestiaryNameIsSet()
        {
            Name = "Shared Bestiary Name";
        }

        [Given]
        public void UserIdIsSet()
        {
            _userId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public async Task UserBestiaryExistsWithSameName()
        {
            // Create a user bestiary with the same name
            await BeastiaryRepository.CreateUserBestiary(_userId, Name, CancellationToken);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnNullDespiteUserBestiaryExisting()
        {
            // Should return null because GetSystemBestiaryByName should only return system bestiaries (OwnerId == null)
            Assert.That(Result, Is.Null);
        }
    }
}