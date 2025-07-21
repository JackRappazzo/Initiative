using Initiative.Import.Core.Services.Extractors;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.ExtractorTests.ConditionImmunitiesExtractorTests
{
    public abstract class WhenTestingConditionImmunitiesExtractor : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected ConditionImmunitiesExtractor ConditionImmunitiesExtractor;
    }
}