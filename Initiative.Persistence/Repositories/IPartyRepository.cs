using Initiative.Persistence.Models.Parties;

namespace Initiative.Persistence.Repositories
{
    public interface IPartyRepository
    {
        Task<string> CreateParty(string ownerId, string name, IEnumerable<PartyMember> members, CancellationToken cancellationToken);
        Task<PartyDocument?> FetchPartyById(string partyId, string ownerId, CancellationToken cancellationToken);
        Task<IEnumerable<PartyDocument>> FetchPartiesByOwnerId(string ownerId, CancellationToken cancellationToken);
        Task<bool> DeleteParty(string partyId, string ownerId, CancellationToken cancellationToken);
    }
}
