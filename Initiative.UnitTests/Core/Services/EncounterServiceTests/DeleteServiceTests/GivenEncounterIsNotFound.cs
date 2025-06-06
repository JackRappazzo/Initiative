using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.DeleteServiceTests
{
    public class GivenEncounterIsNotFound : WhenTestingDeleteEncounter
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterIdIsSet)
            .And(EncounterRepositoryReturnsFalse)
            .When(DeleteIsCalled)
            .Then(ShouldReturnFalse);

        [Given]
        public void EncounterRepositoryReturnsFalse()
        {
            EncounterRepository.DeleteEncounter(EncounterId, CancellationToken)
                .Returns(false);
        }

        [Then]
        public void ShouldReturnFalse()
        {
            Assert.That(Result, Is.False, "Expected the delete operation to return false when the encounter is not found.");
        }
    }
}
