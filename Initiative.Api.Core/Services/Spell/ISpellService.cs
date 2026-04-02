using Initiative.Persistence.Models.Spell;

namespace Initiative.Api.Core.Services.Spell
{
    public class SearchSpellsResult
    {
        public required IEnumerable<SpellDocument> Spells { get; set; }
        public long TotalCount { get; set; }
    }

    public interface ISpellService
    {
        Task<IEnumerable<SpellSourceDocument>> GetAvailableSpellSources(CancellationToken cancellationToken);

        Task<SearchSpellsResult> SearchSpells(SpellSearchQuery query, CancellationToken cancellationToken);

        Task<SpellDocument?> GetSpellById(string spellId, CancellationToken cancellationToken);

        Task<SpellDocument?> GetSpellByNameAndSource(string name, string? source, CancellationToken cancellationToken);
    }
}
