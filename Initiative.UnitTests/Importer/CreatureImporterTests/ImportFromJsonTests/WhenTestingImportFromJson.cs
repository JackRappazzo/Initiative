using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromJsonTests
{
    public abstract class WhenTestingImportFromJson : WhenTestingCreatureImporter
    {
        protected string JsonContent;
        protected List<Creature> Result;

        [When(DoNotRethrowExceptions: true)]
        public void ImportFromJsonIsCalled()
        {
            Result = CreatureImporter.ImportFromJson(JsonContent);
        }
    }
}