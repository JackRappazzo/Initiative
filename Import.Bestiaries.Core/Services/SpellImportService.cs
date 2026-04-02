using Import.Bestiaries.Core.Parsing;
using Initiative.Persistence.Models.Spell;
using Initiative.Persistence.Repositories;

namespace Import.Bestiaries.Core.Services
{
    public class SpellImportService : ISpellImportService
    {
        private readonly ISpellRepository _repository;
        private readonly ISpellParser _parser;
        private readonly ISourceProvider _sourceProvider;

        public SpellImportService(
            ISpellRepository repository,
            ISpellParser parser,
            ISourceProvider sourceProvider)
        {
            _repository = repository;
            _parser = parser;
            _sourceProvider = sourceProvider;
        }

        public async Task ImportAsync(string spellSource, string spellSourceName, string resourceFileName, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetSpellSourceBySource(spellSource, cancellationToken);

            string spellSourceId;
            if (existing is null)
            {
                Console.WriteLine($"Creating spell source '{spellSourceName}' (source: {spellSource})...");
                var spellSourceDoc = new SpellSourceDocument
                {
                    Name = spellSourceName,
                    Source = spellSource
                };
                spellSourceId = await _repository.CreateSpellSource(spellSourceDoc, cancellationToken);
                Console.WriteLine($"Spell source created with id: {spellSourceId}");
            }
            else
            {
                spellSourceId = existing.Id;
                Console.WriteLine($"Spell source '{spellSourceName}' already exists (id: {spellSourceId}). Upserting spells...");
            }

            List<SpellDocument> spells;
            using (var stream = _sourceProvider.OpenSource(resourceFileName))
            {
                spells = _parser.Parse(spellSourceId, spellSource, stream);
            }

            Console.WriteLine($"Parsed {spells.Count} spells. Upserting...");
            await _repository.UpsertSpells(spells, cancellationToken);
            Console.WriteLine("Import complete.");
        }
    }
}
