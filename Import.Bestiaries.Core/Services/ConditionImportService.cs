using Import.Bestiaries.Core.Parsing;
using Initiative.Persistence.Models.Condition;
using Initiative.Persistence.Repositories;

namespace Import.Bestiaries.Core.Services
{
    public class ConditionImportService : IConditionImportService
    {
        private readonly IConditionRepository _repository;
        private readonly IConditionParser _parser;
        private readonly ISourceProvider _sourceProvider;

        public ConditionImportService(
            IConditionRepository repository,
            IConditionParser parser,
            ISourceProvider sourceProvider)
        {
            _repository = repository;
            _parser = parser;
            _sourceProvider = sourceProvider;
        }

        public async Task ImportAsync(string source, string resourceFileName, CancellationToken cancellationToken)
        {
            List<ConditionDocument> conditions;
            using (var stream = _sourceProvider.OpenSource(resourceFileName))
            {
                conditions = _parser.Parse(source, stream);
            }

            Console.WriteLine($"Parsed {conditions.Count} conditions. Upserting...");
            await _repository.UpsertConditions(conditions, cancellationToken);
            Console.WriteLine("Condition import complete.");
        }
    }
}
