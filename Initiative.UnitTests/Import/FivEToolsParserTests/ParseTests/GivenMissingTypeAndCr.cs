using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Import.FivEToolsParserTests.ParseTests
{
    public class GivenMissingTypeAndCr : WhenTestingParse
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(InputCreatureHasNoTypeOrCr)
            .When(ParseIsCalled)
            .Then(ShouldReturnCreatureWithNullTypeAndCr);

        [Given]
        public void InputCreatureHasNoTypeOrCr()
        {
            InputStream = JsonStream("""
                {
                  "monster": [
                    { "name": "Unknown Entity" }
                  ]
                }
                """);
        }

        [Then]
        public void ShouldReturnCreatureWithNullTypeAndCr()
        {
            Assert.That(Result, Has.Count.EqualTo(1));
            Assert.That(Result[0].CreatureType, Is.Null);
            Assert.That(Result[0].ChallengeRating, Is.Null);
        }
    }
}
