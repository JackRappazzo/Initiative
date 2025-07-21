using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SystemNameGeneratorTests
{
    public class GivenSimpleName : WhenTestingGenerateSystemName
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameIsSimple)
            .When(GenerateSystemNameIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldConvertToLowercase)
            .And(ShouldReplaceSpacesWithDashes);

        [Given]
        public void NameIsSimple()
        {
            Name = "Goblin Warrior";
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldConvertToLowercase()
        {
            Assert.That(Result.ToLowerInvariant(), Is.EqualTo(Result));
        }

        [Then]
        public void ShouldReplaceSpacesWithDashes()
        {
            Assert.That(Result, Is.EqualTo("goblin-warrior"));
        }
    }
}