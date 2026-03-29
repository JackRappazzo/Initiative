using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Import.FivEToolsParserTests.ParseTests
{
    public class GivenChooseTypeObject : WhenTestingParse
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(InputHasChooseTypeObject)
            .When(ParseIsCalled)
            .Then(ShouldReturnNullCreatureType);

        [Given]
        public void InputHasChooseTypeObject()
        {
            InputStream = JsonStream("""
                {
                  "monster": [
                    {
                      "name": "Empyrean",
                      "type": {
                        "type": { "choose": ["celestial", "fiend"] },
                        "tags": ["titan"]
                      },
                      "cr": "23"
                    }
                  ]
                }
                """);
        }

        [Then]
        public void ShouldReturnNullCreatureType()
        {
            Assert.That(Result[0].CreatureType, Is.Null);
        }
    }
}
