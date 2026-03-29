using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Import.FivEToolsParserTests.ParseTests
{
    public class GivenNonLegendaryCreature : WhenTestingParse
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(InputHasNoLegendaryProperty)
            .When(ParseIsCalled)
            .Then(ShouldSetIsLegendaryFalse);

        [Given]
        public void InputHasNoLegendaryProperty()
        {
            InputStream = JsonStream("""
                {
                  "monster": [
                    { "name": "Goblin", "type": "humanoid", "cr": "1/4" }
                  ]
                }
                """);
        }

        [Then]
        public void ShouldSetIsLegendaryFalse()
        {
            Assert.That(Result[0].IsLegendary, Is.False);
        }
    }
}
