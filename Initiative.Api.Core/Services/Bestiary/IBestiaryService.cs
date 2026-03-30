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
        /// Searches creatures using the supplied filters.
        /// </summary>
        Task<IEnumerable<BestiaryCreatureDocument>> SearchCreatures(BestiarySearchQuery query, CancellationToken cancellationToken);
    }
}
