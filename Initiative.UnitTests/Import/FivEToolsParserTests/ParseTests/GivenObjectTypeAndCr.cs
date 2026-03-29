using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Import.FivEToolsParserTests.ParseTests
{
    public class GivenObjectTypeAndCr : WhenTestingParse
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(InputIsCreatureWithObjectTypeAndCr)
            .When(ParseIsCalled)
            .Then(ShouldExtractTypeFromNestedObject)
            .And(ShouldExtractCrFromNestedObject);

        [Given]
        public void InputIsCreatureWithObjectTypeAndCr()
        {
            InputStream = JsonStream("""
                {
                  "monster": [
                    {
                      "name": "Ancient Dragon",
                      "type": { "type": "dragon", "tags": ["chromatic"] },
                      "cr": { "cr": "20", "xpLair": 25000 }
                    }
                  ]
                }
                """);
        }

        [Then]
        public void ShouldExtractTypeFromNestedObject()
        {
            Assert.That(Result[0].CreatureType, Is.EqualTo("dragon"));
        }

        [Then]
        public void ShouldExtractCrFromNestedObject()
        {
            Assert.That(Result[0].ChallengeRating, Is.EqualTo("20"));
        }
    }
}
