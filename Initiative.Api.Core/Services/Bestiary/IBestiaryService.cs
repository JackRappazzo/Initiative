using Initiative.Persistence.Models.Bestiary;

namespace Initiative.Api.Core.Services.Bestiary
{
    public interface IBestiaryService
    {
        /// <summary>
        /// Returns all system bestiaries plus any custom bestiaries owned by the given user.
        /// </summary>
        Task<IEnumerable<BestiaryDocument>> GetAvailableBestiaries(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Searches creatures using the supplied filters and returns the matching page together
        /// with the total count of all matching creatures (for pagination).
        /// </summary>
        Task<SearchCreaturesResult> SearchCreatures(BestiarySearchQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a single creature by its ID, or null if not found.
        /// </summary>
        Task<BestiaryCreatureDocument?> GetCreatureById(string creatureId, CancellationToken cancellationToken);
    }
}
