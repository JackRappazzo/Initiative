using Initiative.Import.Core.Services.Extractors;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpeedExtractorTests
{
    public abstract class WhenTestingSpeedExtractor : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected SpeedExtractor SpeedExtractor;
    }
}