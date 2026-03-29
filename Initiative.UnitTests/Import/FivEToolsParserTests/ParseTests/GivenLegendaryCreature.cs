using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Import.FivEToolsParserTests.ParseTests
{
    public class GivenLegendaryCreature : WhenTestingParse
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(InputHasLegendaryProperty)
            .When(ParseIsCalled)
            .Then(ShouldSetIsLegendaryTrue);

        [Given]
        public void InputHasLegendaryProperty()
        {
            InputStream = JsonStream("""
                {
                  "monster": [
                    {
                      "name": "Lich",
                      "type": "undead",
                      "cr": "21",
                      "legendary": [{ "name": "Legendary Resistance" }]
                    }
                  ]
                }
                """);
        }

        [Then]
        public void ShouldSetIsLegendaryTrue()
        {
            Assert.That(Result[0].IsLegendary, Is.True);
        }
    }
}
