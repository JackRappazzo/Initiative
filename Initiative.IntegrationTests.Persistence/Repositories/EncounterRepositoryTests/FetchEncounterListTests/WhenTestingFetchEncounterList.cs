using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters.Dtos;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests.FetchEncounterListTests
{
    public abstract class WhenTestingFetchEncounterList : WhenTestingEncounterRepository
    {
        protected IEnumerable<EncounterListItemDto> Results;
        protected string UserId;

        [When]
        public async Task FetchEncounterListIsCalled()
        {
            Results = await EncounterRepository.FetchEncounterListByUserId(UserId, CancellationToken);
        }
    }
}
