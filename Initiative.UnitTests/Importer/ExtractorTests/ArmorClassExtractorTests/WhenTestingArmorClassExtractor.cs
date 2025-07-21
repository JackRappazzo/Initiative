using Initiative.Import.Core.Services.Extractors;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.ExtractorTests.ArmorClassExtractorTests
{
    public abstract class WhenTestingArmorClassExtractor : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected ArmorClassExtractor ArmorClassExtractor;
    }
}