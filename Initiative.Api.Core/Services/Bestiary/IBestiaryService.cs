using Initiative.Persistence.Models.Bestiary;

namespace Initiative.Api.Core.Services.Bestiary
{
    public class CustomCreatureData
    {
        public required string Name { get; set; }
        public string? CreatureType { get; set; }
        public string? ChallengeRating { get; set; }
        public bool IsLegendary { get; set; }
        public int? HP { get; set; }
        public int? AC { get; set; }
        public string? Traits { get; set; }
    }

    public interface IBestiaryService
    {
        Task<IEnumerable<BestiaryDocument>> GetAvailableBestiaries(string userId, CancellationToken cancellationToken);
        Task<SearchCreaturesResult> SearchCreatures(BestiarySearchQuery query, CancellationToken cancellationToken);
        Task<BestiaryCreatureDocument?> GetCreatureById(string creatureId, CancellationToken cancellationToken);

        Task<BestiaryDocument> CreateCustomBestiary(string userId, string name, CancellationToken cancellationToken);
        Task RenameBestiary(string bestiaryId, string userId, string name, CancellationToken cancellationToken);
        Task DeleteBestiary(string bestiaryId, string userId, CancellationToken cancellationToken);
        Task<BestiaryCreatureDocument> CreateCustomCreature(string bestiaryId, string userId, CustomCreatureData data, CancellationToken cancellationToken);
        Task UpdateCustomCreature(string creatureId, string bestiaryId, string userId, CustomCreatureData data, CancellationToken cancellationToken);
        Task DeleteCustomCreature(string creatureId, CancellationToken cancellationToken);
    }
}
