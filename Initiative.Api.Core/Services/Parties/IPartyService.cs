using Initiative.Persistence.Models.Parties;

namespace Initiative.Api.Core.Services.Parties
{
    public interface IPartyService
    {
        Task<string> CreateParty(string ownerId, string name, IEnumerable<PartyMember> members, CancellationToken cancellationToken);
        Task<PartyDocument?> GetParty(string partyId, string ownerId, CancellationToken cancellationToken);
        Task<IEnumerable<PartyDocument>> GetParties(string ownerId, CancellationToken cancellationToken);
        Task<bool> DeleteParty(string partyId, string ownerId, CancellationToken cancellationToken);
    }
}
