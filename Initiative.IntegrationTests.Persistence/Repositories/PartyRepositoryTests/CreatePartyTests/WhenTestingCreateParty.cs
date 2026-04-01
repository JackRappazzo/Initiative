using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.PartyRepositoryTests.CreatePartyTests
{
    public abstract class WhenTestingCreateParty : WhenTestingPartyRepository
    {
        protected string Result;
        protected string OwnerId;
        protected string Name;
        protected IEnumerable<PartyMember> Members;

        [When]
        public async Task CreatePartyIsCalled()
        {
            Result = await PartyRepository.CreateParty(OwnerId, Name, Members, CancellationToken);
        }
    }
}
