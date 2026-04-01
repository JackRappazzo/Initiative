using Initiative.Persistence.Models.Parties;
using Initiative.Persistence.Repositories;

namespace Initiative.Api.Core.Services.Parties
{
    public class PartyService : IPartyService
    {
        private readonly IPartyRepository partyRepository;

        public PartyService(IPartyRepository partyRepository)
        {
            this.partyRepository = partyRepository;
        }

        public async Task<string> CreateParty(string ownerId, string name, IEnumerable<PartyMember> members, CancellationToken cancellationToken)
        {
            return await partyRepository.CreateParty(ownerId, name, members, cancellationToken);
        }

        public async Task<PartyDocument?> GetParty(string partyId, string ownerId, CancellationToken cancellationToken)
        {
            return await partyRepository.FetchPartyById(partyId, ownerId, cancellationToken);
        }

        public async Task<IEnumerable<PartyDocument>> GetParties(string ownerId, CancellationToken cancellationToken)
        {
            return await partyRepository.FetchPartiesByOwnerId(ownerId, cancellationToken);
        }

        public async Task<bool> DeleteParty(string partyId, string ownerId, CancellationToken cancellationToken)
        {
            return await partyRepository.DeleteParty(partyId, ownerId, cancellationToken);
        }
    }
}
