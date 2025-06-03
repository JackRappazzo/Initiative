using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.CreateEncounterTests
{
    public abstract class WhenTestingCreateEncounter : WhenTestingEncounterService
    {
        protected string OwnerId;
        protected string EncounterName;
        protected string Result;

        [When]
        public async Task CreateEncounterIsCalled()
        {
            Result = await EncounterService.CreateEncounter(OwnerId, EncounterName, CancellationToken);
        }
        [Then]

        public void ResultShouldNotBeNullOrEmpty()
        {
            Assert.That(string.IsNullOrEmpty(Result), Is.False, "Result should not be null or empty.");
        }
    }
}
