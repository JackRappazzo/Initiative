using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.SetCreaturesTests
{
    public abstract class WhenTestingSetCreatures : WhenTestingEncounterRepository
    {
        protected string EncounterId;
        protected IEnumerable<Creature> Creatures;

        [When]
        public async Task SetCreaturesIsCalled()
        {
            await EncounterRepository.SetEncounterCreatures(EncounterId, Creatures, CancellationToken);
        }
    }
}
