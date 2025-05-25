using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.AddCreatureTests
{
    public abstract class WhenTestingAddCreature : WhenTestingBeastiaryRepository
    {
        protected Creature Creature;
        protected string BeastiaryId;

        protected string Result;


        [When]
        public async Task AddCreatureIsCalled()
        {
           Result = await BeastiaryRepository.AddCreature(BeastiaryId, Creature, CancellationToken);
        }
    }
}
