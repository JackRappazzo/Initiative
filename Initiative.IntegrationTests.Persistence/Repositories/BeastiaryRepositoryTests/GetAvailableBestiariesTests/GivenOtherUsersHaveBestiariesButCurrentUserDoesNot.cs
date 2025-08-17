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
    public class GivenOtherUsersHaveBestiariesButCurrentUserDoesNot : WhenTestingGetAvailableBestiaries
    {
        private string _otherUserId;
        private string _otherUserBestiaryId;
        private string _systemBestiaryId;
        private string _systemBestiaryName;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .And(OtherUserHasBestiary)
            .And(SystemBestiaryExists)
            .When(GetAvailableBestiariesIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnOnlySystemBestiaries)
            .And(ShouldNotReturnOtherUsersBestiaries);

        [Given]
        public void UserIdIsSet()
        {
            UserId = "currentUser";
            _otherUserId = "otherUser";
        }

        [Given]
        public async Task OtherUserHasBestiary()
        {
            _otherUserBestiaryId = await BeastiaryRepository.CreateUserBestiary(_otherUserId, "Other User's Bestiary", CancellationToken);
        }

        [Given]
        public async Task SystemBestiaryExists()
        {
            _systemBestiaryName = "Shared System Bestiary";
            _systemBestiaryId = await BeastiaryRepository.CreateSystemBestiary(_systemBestiaryName, CancellationToken);
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
        }

        [Then]
        public void ShouldNotReturnOtherUsersBestiaries()
        {
            Assert.That(Result.Any(b => b.Id == _otherUserBestiaryId), Is.False, "Should not return other users' bestiaries");
            Assert.That(Result.Any(b => b.OwnerId == _otherUserId), Is.False, "Should not return any bestiaries owned by other users");
        }
    }
}