using Import.Bestiaries.Core.Parsing;
using Initiative.Persistence.Models.Bestiary;
using Initiative.Persistence.Repositories;

namespace Import.Bestiaries.Core.Services
{
    public class BestiaryImportService : IBestiaryImportService
    {
        private const string BestiarySource = "XMM";
        private const string BestiaryName = "Monster Manual (2025)";

        private readonly IBestiaryRepository _repository;
        private readonly IFivEToolsParser _parser;
        private readonly ISourceProvider _sourceProvider;

        public BestiaryImportService(
            IBestiaryRepository repository,
            IFivEToolsParser parser,
            ISourceProvider sourceProvider)
        {
            _repository = repository;
            _parser = parser;
            _sourceProvider = sourceProvider;
        }

        public async Task ImportAsync(CancellationToken cancellationToken)
        {
            var existing = await _repository.GetBestiaryBySource(BestiarySource, cancellationToken);

            string bestiaryId;
            if (existing is null)
            {
                Console.WriteLine($"Creating bestiary '{BestiaryName}' (source: {BestiarySource})...");
                var bestiary = new BestiaryDocument
                {
                    Name = BestiaryName,
                    Source = BestiarySource
                };
                bestiaryId = await _repository.CreateBestiary(bestiary, cancellationToken);
                Console.WriteLine($"Bestiary created with id: {bestiaryId}");
            }
            else
            {
                bestiaryId = existing.Id;
                Console.WriteLine($"Bestiary '{BestiaryName}' already exists (id: {bestiaryId}). Upserting creatures...");
            }

            List<BestiaryCreatureDocument> creatures;
            using (var stream = _sourceProvider.OpenMonsterManual25())
            {
                creatures = _parser.Parse(bestiaryId, BestiarySource, stream);
            }

            Console.WriteLine($"Parsed {creatures.Count} creatures. Upserting...");
            await _repository.UpsertCreatures(creatures, cancellationToken);
            Console.WriteLine("Import complete.");
        }
    }
}
