using Initiative.Import.Core.Services.Extractors;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.ExtractorTests.InitiativeCalculatorTests
{
    public abstract class WhenTestingInitiativeCalculator : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected InitiativeCalculator InitiativeCalculator;
    }
}