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
    public class GivenUserHasNoBestiariesButSystemBestiariesExist : WhenTestingGetAvailableBestiaries
    {
        private string _systemBestiaryId1;
        private string _systemBestiaryId2;
        private string _systemBestiaryName1;
        private string _systemBestiaryName2;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .And(SystemBestiariesExist)
            .When(GetAvailableBestiariesIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnOnlySystemBestiaries)
            .And(ShouldReturnCorrectBestiaryData);

        [Given]
        public void UserIdIsSet()
        {
            UserId = "user123";
        }

        [Given]
        public async Task SystemBestiariesExist()
        {
            _systemBestiaryName1 = "System Bestiary 1";
            _systemBestiaryName2 = "System Bestiary 2";

            _systemBestiaryId1 = await BeastiaryRepository.CreateSystemBestiary(_systemBestiaryName1, CancellationToken);
            _systemBestiaryId2 = await BeastiaryRepository.CreateSystemBestiary(_systemBestiaryName2, CancellationToken);
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
            Assert.That(Result.Count(), Is.EqualTo(2));
            Assert.That(Result.All(b => b.OwnerId == null), Is.True, "All returned bestiaries should be system bestiaries (OwnerId == null)");
        }

        [Then]
        public void ShouldReturnCorrectBestiaryData()
        {
            var resultList = Result.ToList();
            
            var bestiary1 = resultList.FirstOrDefault(b => b.Id == _systemBestiaryId1);
            Assert.That(bestiary1, Is.Not.Null);
            Assert.That(bestiary1.Name, Is.EqualTo(_systemBestiaryName1));
            Assert.That(bestiary1.OwnerId, Is.Null);

            var bestiary2 = resultList.FirstOrDefault(b => b.Id == _systemBestiaryId2);
            Assert.That(bestiary2, Is.Not.Null);
            Assert.That(bestiary2.Name, Is.EqualTo(_systemBestiaryName2));
            Assert.That(bestiary2.OwnerId, Is.Null);
        }
    }
}