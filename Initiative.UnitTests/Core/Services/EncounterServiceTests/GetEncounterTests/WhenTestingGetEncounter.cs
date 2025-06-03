using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.GetEncounterTests
{
    public abstract class WhenTestingGetEncounter : WhenTestingEncounterService
    {
        protected string EncounterId;
        protected string OwnerId;
        protected Encounter Result;

        [When]
        public async Task FetchEncounterIsCalled()
        {
            Result = await EncounterService.GetEncounter(EncounterId, OwnerId, CancellationToken);
        }

        [Then]
        public void ShouldNotBeNull()
        {
            Assert.That(Result, Is.Not.Null, "Result should not be null or empty.");
        }
    }
}
