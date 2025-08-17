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
    public class GivenComplexScenarioWithMultipleUsersAndBestiaries : WhenTestingGetAvailableBestiaries
    {
        private string _user1BestiaryId1;
        private string _user1BestiaryId2;
        private string _user2BestiaryId;
        private string _systemBestiaryId1;
        private string _systemBestiaryId2;
        
        private string _user1Id;
        private string _user2Id;
        
        private string _user1BestiaryName1;
        private string _user1BestiaryName2;
        private string _user2BestiaryName;
        private string _systemBestiaryName1;
        private string _systemBestiaryName2;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdsAreSet)
            .And(User1BestiariesExist)
            .And(User2BestiaryExists)
            .And(SystemBestiariesExist)
            .And(TargetUserIsSetToUser1)
            .When(GetAvailableBestiariesIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnCorrectNumberOfBestiaries)
            .And(ShouldReturnUser1BestiariesAndSystemBestiariesOnly)
            .And(ShouldNotReturnUser2Bestiaries)
            .And(ShouldReturnCorrectBestiaryData);

        [Given]
        public void UserIdsAreSet()
        {
            _user1Id = "user1";
            _user2Id = "user2";
        }

        [Given]
        public async Task User1BestiariesExist()
        {
            _user1BestiaryName1 = "User1 Custom Bestiary 1";
            _user1BestiaryName2 = "User1 Custom Bestiary 2";

            _user1BestiaryId1 = await BeastiaryRepository.CreateUserBestiary(_user1Id, _user1BestiaryName1, CancellationToken);
            _user1BestiaryId2 = await BeastiaryRepository.CreateUserBestiary(_user1Id, _user1BestiaryName2, CancellationToken);
        }

        [Given]
        public async Task User2BestiaryExists()
        {
            _user2BestiaryName = "User2 Private Bestiary";
            _user2BestiaryId = await BeastiaryRepository.CreateUserBestiary(_user2Id, _user2BestiaryName, CancellationToken);
        }

        [Given]
        public async Task SystemBestiariesExist()
        {
            _systemBestiaryName1 = "Official Monster Manual";
            _systemBestiaryName2 = "Player's Handbook Creatures";

            _systemBestiaryId1 = await BeastiaryRepository.CreateSystemBestiary(_systemBestiaryName1, CancellationToken);
            _systemBestiaryId2 = await BeastiaryRepository.CreateSystemBestiary(_systemBestiaryName2, CancellationToken);
        }

        [Given]
        public void TargetUserIsSetToUser1()
        {
            UserId = _user1Id;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnCorrectNumberOfBestiaries()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Count(), Is.EqualTo(4), "Should return 2 user bestiaries + 2 system bestiaries = 4 total");
        }

        [Then]
        public void ShouldReturnUser1BestiariesAndSystemBestiariesOnly()
        {
            var user1Bestiaries = Result.Where(b => b.OwnerId == _user1Id).ToList();
            var systemBestiaries = Result.Where(b => b.OwnerId == null).ToList();

            Assert.That(user1Bestiaries.Count, Is.EqualTo(2), "Should return exactly 2 user bestiaries for user1");
            Assert.That(systemBestiaries.Count, Is.EqualTo(2), "Should return exactly 2 system bestiaries");
        }

        [Then]
        public void ShouldNotReturnUser2Bestiaries()
        {
            Assert.That(Result.Any(b => b.OwnerId == _user2Id), Is.False, "Should not return any bestiaries owned by user2");
            Assert.That(Result.Any(b => b.Id == _user2BestiaryId), Is.False, "Should not return user2's specific bestiary");
        }

        [Then]
        public void ShouldReturnCorrectBestiaryData()
        {
            var resultList = Result.ToList();

            // Verify User1's bestiaries
            var user1Bestiary1 = resultList.FirstOrDefault(b => b.Id == _user1BestiaryId1);
            Assert.That(user1Bestiary1, Is.Not.Null);
            Assert.That(user1Bestiary1.Name, Is.EqualTo(_user1BestiaryName1));
            Assert.That(user1Bestiary1.OwnerId, Is.EqualTo(_user1Id));

            var user1Bestiary2 = resultList.FirstOrDefault(b => b.Id == _user1BestiaryId2);
            Assert.That(user1Bestiary2, Is.Not.Null);
            Assert.That(user1Bestiary2.Name, Is.EqualTo(_user1BestiaryName2));
            Assert.That(user1Bestiary2.OwnerId, Is.EqualTo(_user1Id));

            // Verify System bestiaries
            var systemBestiary1 = resultList.FirstOrDefault(b => b.Id == _systemBestiaryId1);
            Assert.That(systemBestiary1, Is.Not.Null);
            Assert.That(systemBestiary1.Name, Is.EqualTo(_systemBestiaryName1));
            Assert.That(systemBestiary1.OwnerId, Is.Null);

            var systemBestiary2 = resultList.FirstOrDefault(b => b.Id == _systemBestiaryId2);
            Assert.That(systemBestiary2, Is.Not.Null);
            Assert.That(systemBestiary2.Name, Is.EqualTo(_systemBestiaryName2));
            Assert.That(systemBestiary2.OwnerId, Is.Null);
        }
    }
}