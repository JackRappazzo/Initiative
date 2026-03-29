using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Import.FivEToolsParserTests.ParseTests
{
    public class GivenStringTypeAndCr : WhenTestingParse
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(InputIsCreatureWithStringTypeAndCr)
            .When(ParseIsCalled)
            .Then(ShouldReturnOneCreature)
            .And(ShouldMapNameCorrectly)
            .And(ShouldMapTypeCorrectly)
            .And(ShouldMapCrCorrectly)
            .And(ShouldSetBestiaryIdAndSource)
            .And(ShouldPopulateRawData);

        [Given]
        public void InputIsCreatureWithStringTypeAndCr()
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
        public void ShouldReturnOneCreature()
        {
            Assert.That(Result, Has.Count.EqualTo(1));
        }

        [Then]
        public void ShouldMapNameCorrectly()
        {
            Assert.That(Result[0].Name, Is.EqualTo("Goblin"));
        }

        [Then]
        public void ShouldMapTypeCorrectly()
        {
            Assert.That(Result[0].CreatureType, Is.EqualTo("humanoid"));
        }

        [Then]
        public void ShouldMapCrCorrectly()
        {
            Assert.That(Result[0].ChallengeRating, Is.EqualTo("1/4"));
        }

        [Then]
        public void ShouldSetBestiaryIdAndSource()
        {
            Assert.That(Result[0].BestiaryId, Is.EqualTo(BestiaryId));
            Assert.That(Result[0].Source, Is.EqualTo(Source));
        }

        [Then]
        public void ShouldPopulateRawData()
        {
            Assert.That(Result[0].RawData, Is.Not.Null);
            Assert.That(Result[0].RawData.Contains("name"), Is.True);
        }
    }
}
