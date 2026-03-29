using Import.Bestiaries.Core.Parsing;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Import.FivEToolsParserTests
{
    public abstract class WhenTestingFivEToolsParser : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected FivEToolsParser Parser;
    }
}
