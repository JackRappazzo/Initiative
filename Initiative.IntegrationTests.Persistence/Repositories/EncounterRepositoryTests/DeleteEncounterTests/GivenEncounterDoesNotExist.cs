using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.DeleteEncounterTests
{
    public class GivenEncounterDoesNotExist : WhenTestingDeleteEncounter
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterDoesNotExist)
            .When(DeleteEncounterIsCalled)
            .Then(ShouldReturnFalse);


        [Given]
        public void EncounterDoesNotExist()
        {
            // No setup needed, as we are testing the case where the encounter does not exist.
        }

        [Then]
        public void ShouldReturnFalse()
        {
            Assert.That(Result, Is.False, "Encounter deletion should return false when the encounter does not exist.");
        }
    }
}
