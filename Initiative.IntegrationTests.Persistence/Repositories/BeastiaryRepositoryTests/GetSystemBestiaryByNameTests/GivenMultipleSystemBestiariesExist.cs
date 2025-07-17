using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.GetSystemBestiaryByNameTests
{
    public class GivenMultipleSystemBestiariesExist : WhenTestingGetSystemBestiaryByName
    {
        private string _targetBestiaryId;
        private string _otherBestiaryId1;
        private string _otherBestiaryId2;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryNameIsSet)
            .And(MultipleSystemBestiariesExist)
            .When(GetSystemBestiaryByNameIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnCorrectBestiary)
            .And(ShouldNotReturnOtherBestiaries);

        [Given]
        public void BestiaryNameIsSet()
        {
            Name = "Target Bestiary";
        }

        [Given]
        public async Task MultipleSystemBestiariesExist()
        {
            // Create the target bestiary
            _targetBestiaryId = await BeastiaryRepository.CreateSystemBestiary(Name, CancellationToken);
            
            // Create other bestiaries with different names
            _otherBestiaryId1 = await BeastiaryRepository.CreateSystemBestiary("Other Bestiary 1", CancellationToken);
            _otherBestiaryId2 = await BeastiaryRepository.CreateSystemBestiary("Different Bestiary", CancellationToken);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnCorrectBestiary()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Id, Is.EqualTo(_targetBestiaryId));
            Assert.That(Result.Name, Is.EqualTo(Name));
        }

        [Then]
        public void ShouldNotReturnOtherBestiaries()
        {
            Assert.That(Result.Id, Is.Not.EqualTo(_otherBestiaryId1));
            Assert.That(Result.Id, Is.Not.EqualTo(_otherBestiaryId2));
        }
    }
}