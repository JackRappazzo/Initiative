using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SystemNameGeneratorTests
{
    public class GivenNameWithSpecialCharacters : WhenTestingGenerateSystemName
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameHasSpecialCharacters)
            .When(GenerateSystemNameIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldRemoveSpecialCharacters);

        [Given]
        public void NameHasSpecialCharacters()
        {
            Name = "Tiamat's Dragon (Ancient Red)";
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldRemoveSpecialCharacters()
        {
            Assert.That(Result, Is.EqualTo("tiamats-dragon-ancient-red"));
            Assert.That(Result, Does.Not.Contain("'"));
            Assert.That(Result, Does.Not.Contain("("));
            Assert.That(Result, Does.Not.Contain(")"));
        }
    }
}