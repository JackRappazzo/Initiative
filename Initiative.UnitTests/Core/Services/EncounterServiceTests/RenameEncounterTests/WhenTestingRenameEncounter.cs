using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.RenameEncounterTests
{
    public abstract class WhenTestingRenameEncounter : WhenTestingEncounterService
    {
        protected string EncounterId;
        protected string OwnerId;
        protected string NewName;

        [When]
        public async Task RenameEncounterIsCalled()
        {
            await EncounterService.RenameEncounter(EncounterId, OwnerId, NewName, CancellationToken);
        }
    }
}
