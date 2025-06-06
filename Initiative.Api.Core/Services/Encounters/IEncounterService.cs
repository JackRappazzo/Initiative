
using Initiative.Persistence.Models.Encounters;

namespace Initiative.Api.Core.Services.Encounters
{
    public interface IEncounterService
    {
        Task<string> CreateEncounter(string ownerId, string encounterName, CancellationToken cancellationToken);
        Task<IEnumerable<EncounterListItem>> GetEncounterList(string ownerId, CancellationToken cancellationToken);
        Task<Encounter> GetEncounter(string encounterId, string ownerId, CancellationToken cancellationToken);
        Task SetEncounterCreatures(string encounterId, string ownerId, IEnumerable<Creature> creatures, CancellationToken cancellationToken);
        Task RenameEncounter(string encounterId, string ownerId, string name, CancellationToken cancellationToken);
        Task<bool> DeleteEncounter(string encounterId, CancellationToken cancellationToken);
    }
}