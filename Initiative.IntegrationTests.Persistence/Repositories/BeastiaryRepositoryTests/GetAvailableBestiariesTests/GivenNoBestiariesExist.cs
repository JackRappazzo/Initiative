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
    public class GivenNoBestiariesExist : WhenTestingGetAvailableBestiaries
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .When(GetAvailableBestiariesIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnEmptyList);

        [Given]
        public void UserIdIsSet()
        {
            UserId = "user789";
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnEmptyList()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Count(), Is.EqualTo(0));
        }
    }
}