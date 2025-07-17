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
    public class GivenBestiaryDoesNotExist : WhenTestingGetSystemBestiaryByName
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NonExistentBestiaryNameIsSet)
            .When(GetSystemBestiaryByNameIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnNull);

        [Given]
        public void NonExistentBestiaryNameIsSet()
        {
            Name = "Non-Existent Bestiary " + Guid.NewGuid();
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnNull()
        {
            Assert.That(Result, Is.Null);
        }
    }
}