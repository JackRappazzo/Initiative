using Import.Bestiaries.Core.Parsing;
using Initiative.Persistence.Models.Bestiary;
using Initiative.Persistence.Repositories;

namespace Import.Bestiaries.Core.Services
{
    public class BestiaryImportService : IBestiaryImportService
    {
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

        public async Task ImportAsync(string bestiarySource, string bestiaryName, string resourceFileName, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetBestiaryBySource(bestiarySource, cancellationToken);

            string bestiaryId;
            if (existing is null)
            {
                Console.WriteLine($"Creating bestiary '{bestiaryName}' (source: {bestiarySource})...");
                var bestiary = new BestiaryDocument
                {
                    Name = bestiaryName,
                    Source = bestiarySource
                };
                bestiaryId = await _repository.CreateBestiary(bestiary, cancellationToken);
                Console.WriteLine($"Bestiary created with id: {bestiaryId}");
            }
            else
            {
                bestiaryId = existing.Id;
                Console.WriteLine($"Bestiary '{bestiaryName}' already exists (id: {bestiaryId}). Upserting creatures...");
            }

            List<BestiaryCreatureDocument> creatures;
            using (var stream = _sourceProvider.OpenSource(resourceFileName))
            {
                creatures = _parser.Parse(bestiaryId, bestiarySource, stream);
            }

            Console.WriteLine($"Parsed {creatures.Count} creatures. Upserting...");
            await _repository.UpsertCreatures(creatures, cancellationToken);
            Console.WriteLine("Import complete.");
        }
    }
}
