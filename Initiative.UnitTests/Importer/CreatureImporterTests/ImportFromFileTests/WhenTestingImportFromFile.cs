using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromFileTests
{
    public abstract class WhenTestingImportFromFile : WhenTestingCreatureImporter
    {
        protected string FilePath;
        protected List<Creature> Result;

        [When(DoNotRethrowExceptions: true)]
        public async Task ImportFromFileAsyncIsCalled()
        {
            Result = await CreatureImporter.ImportFromFileAsync(FilePath, CancellationToken);
        }
    }
}