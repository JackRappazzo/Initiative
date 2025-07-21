using System.Text.Json;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.ConditionImmunitiesExtractorTests
{
    public abstract class WhenTestingExtractConditionImmunities : WhenTestingConditionImmunitiesExtractor
    {
        protected List<JsonElement>? ConditionImmunitiesList;
        protected List<string> Result;

        [When(DoNotRethrowExceptions: true)]
        public void ExtractConditionImmunitiesIsCalled()
        {
            Result = ConditionImmunitiesExtractor.ExtractConditionImmunities(ConditionImmunitiesList);
        }
    }
}