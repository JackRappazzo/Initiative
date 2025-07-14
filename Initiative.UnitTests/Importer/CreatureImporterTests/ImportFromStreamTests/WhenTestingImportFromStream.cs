using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromStreamTests
{
    public abstract class WhenTestingImportFromStream : WhenTestingCreatureImporter
    {
        protected Stream Stream;
        protected List<Creature> Result;

        [When(DoNotRethrowExceptions: true)]
        public async Task ImportFromStreamAsyncIsCalled()
        {
            Result = await CreatureImporter.ImportFromStreamAsync(Stream, CancellationToken);
        }
    }
}