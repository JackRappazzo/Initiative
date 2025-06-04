using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.SetEncounterNameTests
{
    public abstract class WhenTestingSetEncounterName : WhenTestingEncounterRepository
    {
        protected string EncounterId;
        protected string NewName;

        [When]
        public async Task SetEncounterNameIsCalled()
        {
            await EncounterRepository.SetEncounterName(EncounterId, NewName, CancellationToken);
        }

        [Given]
        public void NewNameIsSet()
        {
            NewName = "New Encounter Name " + Guid.NewGuid();
        }
    }
}
