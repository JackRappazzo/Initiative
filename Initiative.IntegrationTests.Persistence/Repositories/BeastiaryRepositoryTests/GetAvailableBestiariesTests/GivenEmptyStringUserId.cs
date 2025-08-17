using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.GetAvailableBestiariesTests
{
    public class GivenEmptyStringUserId : WhenTestingGetAvailableBestiaries
    {
        private string _systemBestiaryId;
        private string _userBestiaryId;
        private string _systemBestiaryName;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsEmptyString)
            .And(SystemAndUserBestiariesExist)
            .When(GetAvailableBestiariesIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnOnlySystemBestiaries);

        [Given]
        public void UserIdIsEmptyString()
        {
            UserId = string.Empty;
        }

        [Given]
        public async Task SystemAndUserBestiariesExist()
        {
            _systemBestiaryName = "Available System Bestiary";
            _systemBestiaryId = await BeastiaryRepository.CreateSystemBestiary(_systemBestiaryName, CancellationToken);
            
            // Create a user bestiary that should not be returned
            _userBestiaryId = await BeastiaryRepository.CreateUserBestiary("validUser", "Private User Bestiary", CancellationToken);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnOnlySystemBestiaries()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Count(), Is.EqualTo(1));
            
            var returnedBestiary = Result.First();
            Assert.That(returnedBestiary.Id, Is.EqualTo(_systemBestiaryId));
            Assert.That(returnedBestiary.Name, Is.EqualTo(_systemBestiaryName));
            Assert.That(returnedBestiary.OwnerId, Is.Null);
            
            // Ensure user bestiary is not returned
            Assert.That(Result.Any(b => b.Id == _userBestiaryId), Is.False, "Should not return user bestiaries when userId is empty string");
        }
    }
}