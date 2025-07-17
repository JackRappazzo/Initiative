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
    public class GivenBestiaryExists : WhenTestingGetSystemBestiaryByName
    {
        private string _createdBestiaryId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryNameIsSet)
            .And(SystemBestiaryExists)
            .When(GetSystemBestiaryByNameIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnCorrectBestiary)
            .And(ShouldHaveCorrectProperties);

        [Given]
        public void BestiaryNameIsSet()
        {
            Name = "Monster Manual";
        }

        [Given]
        public async Task SystemBestiaryExists()
        {
            _createdBestiaryId = await BeastiaryRepository.CreateSystemBestiary(Name, CancellationToken);
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
            Assert.That(Result.Id, Is.EqualTo(_createdBestiaryId));
        }

        [Then]
        public void ShouldHaveCorrectProperties()
        {
            Assert.That(Result.Name, Is.EqualTo(Name));
            Assert.That(Result.OwnerId, Is.Null); // System bestiary should have null OwnerId
            Assert.That(Result.Creatures, Is.Not.Null);
        }
    }
}