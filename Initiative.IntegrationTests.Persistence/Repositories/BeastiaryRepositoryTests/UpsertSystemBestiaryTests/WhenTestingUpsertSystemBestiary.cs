using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.UpsertSystemBestiaryTests
{
    public abstract class WhenTestingUpsertSystemBestiary : WhenTestingBeastiaryRepository
    {
        protected string Name;
        protected IEnumerable<Creature> Creatures;
        protected string Result;

        [When(DoNotRethrowExceptions: true)]
        public async Task UpsertSystemBestiaryIsCalled()
        {
            Result = await BeastiaryRepository.UpsertSystemBestiary(Name, Creatures, CancellationToken);
        }
    }
}