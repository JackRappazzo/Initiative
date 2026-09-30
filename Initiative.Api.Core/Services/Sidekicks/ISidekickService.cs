using Initiative.Persistence.Models.Parties;

namespace Initiative.Api.Core.Services.Sidekicks
{
    public interface ISidekickService
    {
        Task<PartyMember> CreateSidekick(string partyId, string ownerId, string name, SidekickData data, CancellationToken cancellationToken);
        Task<PartyMember> UpdateSidekick(string partyId, string ownerId, string memberId, string name, SidekickData data, CancellationToken cancellationToken);
        Task<bool> RemoveSidekick(string partyId, string ownerId, string memberId, CancellationToken cancellationToken);
    }
}
