using Initiative.Persistence.Models.Spell;
using Initiative.Persistence.Repositories;

namespace Initiative.Api.Core.Services.Spell
{
    public class SpellService : ISpellService
    {
        private readonly ISpellRepository _repository;

        public SpellService(ISpellRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<SpellSourceDocument>> GetAvailableSpellSources(CancellationToken cancellationToken)
            => _repository.GetAllSpellSources(cancellationToken);

        public async Task<SearchSpellsResult> SearchSpells(SpellSearchQuery query, CancellationToken cancellationToken)
        {
            var countQuery = new SpellSearchQuery
            {
                SpellSourceIds = query.SpellSourceIds,
                NameSearch = query.NameSearch,
                School = query.School,
            };

            var spellsTask = _repository.SearchSpells(query, cancellationToken);
            var countTask = _repository.CountSpells(countQuery, cancellationToken);
            await Task.WhenAll(spellsTask, countTask);

            return new SearchSpellsResult { Spells = spellsTask.Result, TotalCount = countTask.Result };
        }

        public Task<SpellDocument?> GetSpellById(string spellId, CancellationToken cancellationToken)
            => _repository.GetSpellById(spellId, cancellationToken);

        public Task<SpellDocument?> GetSpellByNameAndSource(string name, string? source, CancellationToken cancellationToken)
            => _repository.GetSpellByNameAndSource(name, source, cancellationToken);
    }
}
