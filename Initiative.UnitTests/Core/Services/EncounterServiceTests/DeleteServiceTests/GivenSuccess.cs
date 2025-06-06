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
    public class GivenSuccess : WhenTestingDeleteEncounter
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterIdIsSet)
            .And(EncounterRepositoryDeletesEncounter)
            .When(DeleteIsCalled)
            .Then(ShouldReturnTrue);

        [Given]
        public void EncounterRepositoryDeletesEncounter()
        {
            EncounterRepository.DeleteEncounter(EncounterId, CancellationToken)
                .Returns(true);
        }

        [Then]
        public void ShouldReturnTrue()
        {
            Assert.That(Result, Is.True, "Expected the delete operation to return true.");
        }
    }
}
