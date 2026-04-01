using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.PartyRepositoryTests.FetchPartyTests
{
    public abstract class WhenTestingFetchParty : WhenTestingPartyRepository
    {
        protected PartyDocument? Result;
        protected string PartyId;
        protected string OwnerId;

        [When]
        public async Task FetchPartyIsCalled()
        {
            Result = await PartyRepository.FetchPartyById(PartyId, OwnerId, CancellationToken);
        }
    }
}
