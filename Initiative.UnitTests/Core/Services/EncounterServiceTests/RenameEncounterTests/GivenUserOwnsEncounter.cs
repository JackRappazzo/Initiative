using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.RenameEncounterTests
{
    public class GivenUserOwnsEncounter : WhenTestingRenameEncounter
    {

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterIdIsSet)
            .And(OwnerIdIsSet)
            .And(NewNameIsSet)
            .And(RepositoryReturnsEncounter)
            .When(RenameEncounterIsCalled)
            .Then(ShouldCallRenameOnRepository);

        [Given]
        public void EncounterIdIsSet()
        {
            EncounterId = Guid.NewGuid().ToString();
        }
        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = Guid.NewGuid().ToString();
        }
        [Given]
        public void NewNameIsSet()
        {
            NewName = "New Encounter Name";
        }
        [Given]
        public void RepositoryReturnsEncounter()
        {
            EncounterRepository.FetchEncounterById(EncounterId, OwnerId, CancellationToken)
                .Returns(new Persistence.Models.Encounters.Encounter
                {
                    Id = EncounterId,
                    OwnerId = OwnerId,
                    DisplayName = "Old Encounter Name"
                });
        }

        [Then]
        public void ShouldCallRenameOnRepository()
        {
            EncounterRepository.Received(1).SetEncounterName(EncounterId, NewName, CancellationToken);
        }
    }
}
