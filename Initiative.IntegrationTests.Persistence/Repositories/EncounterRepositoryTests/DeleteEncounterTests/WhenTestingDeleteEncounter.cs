using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.DeleteEncounterTests
{
    public abstract class WhenTestingDeleteEncounter : WhenTestingEncounterRepository
    {
        protected bool Result;
        protected string EncounterId;

        [When]
        public async Task DeleteEncounterIsCalled()
        {
            Result = await EncounterRepository.DeleteEncounter(EncounterId, CancellationToken);
        }
    }
}
