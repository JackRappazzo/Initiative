
using Initiative.Persistence.Models.Encounters;

namespace Initiative.Api.Core.Services.Encounters
{
    public interface IEncounterService
    {
        Task<string> CreateEncounter(string ownerId, string encounterName, CancellationToken cancellationToken);
        Task<IEnumerable<EncounterListItem>> GetEncounterList(string ownerId, CancellationToken cancellationToken);
        Task<Encounter> GetEncounter(string encounterId, string ownerId, CancellationToken cancellationToken);
        Task SetEncounterCreatures(string encounterId, string ownerId, IEnumerable<EncounterCreature> creatures, CancellationToken cancellationToken);
        Task RenameEncounter(string encounterId, string ownerId, string name, CancellationToken cancellationToken);
        Task<bool> DeleteEncounter(string encounterId, CancellationToken cancellationToken);
        Task SetEncounterTurnState(string encounterId, string ownerId, int turnIndex, int turnCount, CancellationToken cancellationToken);
        Task SetViewersAllowed(string encounterId, string ownerId, bool viewersAllowed, CancellationToken cancellationToken);
        Task SetEncounterPartyLevels(string encounterId, string ownerId, IEnumerable<int> partyLevels, CancellationToken cancellationToken);
    }
}