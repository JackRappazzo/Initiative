using Initiative.Import.Core.Services.Extractors;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpellcastingExtractorTests
{
    public abstract class WhenTestingSpellcastingExtractor : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected SpellcastingExtractor SpellcastingExtractor;
    }
}