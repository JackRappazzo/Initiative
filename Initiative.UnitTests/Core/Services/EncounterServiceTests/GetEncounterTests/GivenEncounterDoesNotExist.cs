using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.GetEncounterTests
{
    public class GivenEncounterDoesNotExist : WhenTestingGetEncounter
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(OwnerIdIsSet)
            .And(EncounterIdIsSet)
            .And(RepositoryReturnsNull)
            .When(FetchEncounterIsCalled)
            .Then(ResultShouldBeNull);

        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = Guid.NewGuid().ToString();
        }
        [Given]
        public void EncounterIdIsSet()
        {
            EncounterId = Guid.NewGuid().ToString();
        }
        [Given]
        public void RepositoryReturnsNull()
        {
            EncounterRepository.FetchEncounterById(EncounterId, OwnerId, CancellationToken)
                .ReturnsNull();
        }

        [Then]
        public void ResultShouldBeNull()
        {
            Assert.That(Result, Is.Null, "Result should be null when the encounter does not exist.");
        }
    }
}
