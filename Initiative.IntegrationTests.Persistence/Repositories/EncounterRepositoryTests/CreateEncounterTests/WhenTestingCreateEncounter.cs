using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.CreateEncounterTests
{
    public abstract class WhenTestingCreateEncounter : WhenTestingEncounterRepository
    {
        protected string Result;
        protected string Name;
        protected string OwnerId;

        [When]
        public async Task CreateEncounterIsCalled()
        {
            Result = await EncounterRepository.CreateEncounter(OwnerId, Name, CancellationToken);
        }
    }
}
