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
    public class GivenUserHasOwnBestiariesAndSystemBestiariesExist : WhenTestingGetAvailableBestiaries
    {
        private string _userBestiaryId1;
        private string _userBestiaryId2;
        private string _systemBestiaryId;
        private string _userBestiaryName1;
        private string _userBestiaryName2;
        private string _systemBestiaryName;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .And(UserBestiariesExist)
            .And(SystemBestiaryExists)
            .When(GetAvailableBestiariesIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnBothUserAndSystemBestiaries)
            .And(ShouldReturnCorrectBestiaryData);

        [Given]
        public void UserIdIsSet()
        {
            UserId = "user456";
        }

        [Given]
        public async Task UserBestiariesExist()
        {
            _userBestiaryName1 = "User's Custom Bestiary";
            _userBestiaryName2 = "User's Monster Collection";

            _userBestiaryId1 = await BeastiaryRepository.CreateUserBestiary(UserId, _userBestiaryName1, CancellationToken);
            _userBestiaryId2 = await BeastiaryRepository.CreateUserBestiary(UserId, _userBestiaryName2, CancellationToken);
        }

        [Given]
        public async Task SystemBestiaryExists()
        {
            _systemBestiaryName = "Official Monster Manual";
            _systemBestiaryId = await BeastiaryRepository.CreateSystemBestiary(_systemBestiaryName, CancellationToken);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnBothUserAndSystemBestiaries()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Count(), Is.EqualTo(3));
            
            var userBestiaries = Result.Where(b => b.OwnerId == UserId).ToList();
            var systemBestiaries = Result.Where(b => b.OwnerId == null).ToList();
            
            Assert.That(userBestiaries.Count, Is.EqualTo(2), "Should return 2 user bestiaries");
            Assert.That(systemBestiaries.Count, Is.EqualTo(1), "Should return 1 system bestiary");
        }

        [Then]
        public void ShouldReturnCorrectBestiaryData()
        {
            var resultList = Result.ToList();
            
            // Check user bestiaries
            var userBestiary1 = resultList.FirstOrDefault(b => b.Id == _userBestiaryId1);
            Assert.That(userBestiary1, Is.Not.Null);
            Assert.That(userBestiary1.Name, Is.EqualTo(_userBestiaryName1));
            Assert.That(userBestiary1.OwnerId, Is.EqualTo(UserId));

            var userBestiary2 = resultList.FirstOrDefault(b => b.Id == _userBestiaryId2);
            Assert.That(userBestiary2, Is.Not.Null);
            Assert.That(userBestiary2.Name, Is.EqualTo(_userBestiaryName2));
            Assert.That(userBestiary2.OwnerId, Is.EqualTo(UserId));

            // Check system bestiary
            var systemBestiary = resultList.FirstOrDefault(b => b.Id == _systemBestiaryId);
            Assert.That(systemBestiary, Is.Not.Null);
            Assert.That(systemBestiary.Name, Is.EqualTo(_systemBestiaryName));
            Assert.That(systemBestiary.OwnerId, Is.Null);
        }
    }
}