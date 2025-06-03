using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.SetCreaturesTests
{
    public abstract class WhenTestingSetCreatures : WhenTestingEncounterService
    {
        protected string EncounterId;
        protected string OwnerId;
        protected IEnumerable<Creature> Creatures;

        [When(DoNotRethrowExceptions:true)]
        public async Task SetCreaturesIsCalled()
        {
            await EncounterService.SetEncounterCreatures(EncounterId, OwnerId, Creatures, CancellationToken);
        }
    }
}
