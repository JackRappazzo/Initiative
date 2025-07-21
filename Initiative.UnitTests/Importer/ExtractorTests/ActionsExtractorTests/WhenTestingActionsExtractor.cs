using Initiative.Import.Core.Services.Extractors;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.ExtractorTests.ActionsExtractorTests
{
    public abstract class WhenTestingActionsExtractor : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected ActionsExtractor ActionsExtractor;
    }
}