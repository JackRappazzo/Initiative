using Initiative.Persistence.Models.Bestiary;

namespace Initiative.Persistence.Repositories
{
    public interface IBestiaryRepository
    {
        /// <summary>
        /// Creates a new bestiary record and returns its generated ID.
        /// </summary>
        Task<string> CreateBestiary(BestiaryDocument bestiary, CancellationToken cancellationToken);

        /// <summary>
        /// Returns an existing bestiary by its source code (e.g. "MM"), or null if not found.
        /// </summary>
        Task<BestiaryDocument?> GetBestiaryBySource(string source, CancellationToken cancellationToken);

        /// <summary>
        /// Returns all bestiaries, ordered by name.
        /// </summary>
        Task<IEnumerable<BestiaryDocument>> GetAllBestiaries(CancellationToken cancellationToken);

        /// <summary>
        /// Returns all bestiaries owned by the given user, ordered by name.
        /// </summary>
        Task<IEnumerable<BestiaryDocument>> GetBestariesByOwner(string ownerId, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts or replaces creatures belonging to the given bestiary.
        /// Matching is performed on (BestiaryId + Name); existing documents are replaced,
        /// new ones are inserted.
        /// </summary>
        Task UpsertCreatures(IEnumerable<BestiaryCreatureDocument> creatures, CancellationToken cancellationToken);

        /// <summary>
        /// Searches creatures using the supplied query filters.
        /// Supports type-ahead (prefix match on Name), type, CR, legendary, and bestiary filters.
        /// Results are ordered by Name.
        /// </summary>
        Task<IEnumerable<BestiaryCreatureDocument>> SearchCreatures(BestiarySearchQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the total number of creatures matching the supplied query filters,
        /// ignoring the Skip and PageSize pagination fields.
        /// </summary>
        Task<long> CountCreatures(BestiarySearchQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the full creature document by its ID, or null if not found.
        /// </summary>
        Task<BestiaryCreatureDocument?> GetCreatureById(string creatureId, CancellationToken cancellationToken);

        Task<BestiaryDocument?> GetBestiaryById(string bestiaryId, CancellationToken cancellationToken);
        Task RenameBestiary(string bestiaryId, string ownerId, string name, CancellationToken cancellationToken);
        Task DeleteBestiary(string bestiaryId, string ownerId, CancellationToken cancellationToken);
        Task<string> InsertCreature(BestiaryCreatureDocument creature, CancellationToken cancellationToken);
        Task UpdateCreature(BestiaryCreatureDocument creature, CancellationToken cancellationToken);
        Task DeleteCreature(string creatureId, CancellationToken cancellationToken);
    }
}
