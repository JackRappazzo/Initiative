using Initiative.Import.Core.Services.Extractors;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.ExtractorTests.SystemNameGeneratorTests
{
    public abstract class WhenTestingSystemNameGenerator : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected SystemNameGenerator SystemNameGenerator;
    }
}