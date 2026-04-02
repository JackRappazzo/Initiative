using Initiative.Persistence.Models.Spell;

namespace Initiative.Persistence.Repositories
{
    public interface ISpellRepository
    {
        /// <summary>
        /// Creates a new spell source record and returns its generated ID.
        /// </summary>
        Task<string> CreateSpellSource(SpellSourceDocument spellSource, CancellationToken cancellationToken);

        /// <summary>
        /// Returns an existing spell source by its source code (e.g. "XPHB"), or null if not found.
        /// </summary>
        Task<SpellSourceDocument?> GetSpellSourceBySource(string source, CancellationToken cancellationToken);

        /// <summary>
        /// Returns all spell sources, ordered by name.
        /// </summary>
        Task<IEnumerable<SpellSourceDocument>> GetAllSpellSources(CancellationToken cancellationToken);

        /// <summary>
        /// Inserts or updates spells belonging to the given spell source.
        /// Matching is performed on (SpellSourceId + Name); existing documents are updated,
        /// new ones are inserted.
        /// </summary>
        Task UpsertSpells(IEnumerable<SpellDocument> spells, CancellationToken cancellationToken);

        /// <summary>
        /// Searches spells using the supplied query filters.
        /// Supports type-ahead (prefix match on Name), school, and source filters.
        /// Results are ordered by Name.
        /// </summary>
        Task<IEnumerable<SpellDocument>> SearchSpells(SpellSearchQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the total number of spells matching the supplied query filters,
        /// ignoring Skip and PageSize pagination fields.
        /// </summary>
        Task<long> CountSpells(SpellSearchQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the full spell document by its ID, or null if not found.
        /// </summary>
        Task<SpellDocument?> GetSpellById(string spellId, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a spell by its name and source code (case-insensitive name match).
        /// Used to resolve {@spell} placeholders in creature spellcasting blocks.
        /// Source is optional; when null, returns the first matching spell by name.
        /// </summary>
        Task<SpellDocument?> GetSpellByNameAndSource(string name, string? source, CancellationToken cancellationToken);
    }
}
