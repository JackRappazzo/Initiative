using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.ActionsExtractorTests
{
    public abstract class WhenTestingExtractActions : WhenTestingActionsExtractor
    {
        protected List<ActionJson>? Actions;
        protected IEnumerable<CreatureAction> Result;

        [When(DoNotRethrowExceptions: true)]
        public void ExtractActionsIsCalled()
        {
            Result = ActionsExtractor.ExtractActions(Actions);
        }
    }
}