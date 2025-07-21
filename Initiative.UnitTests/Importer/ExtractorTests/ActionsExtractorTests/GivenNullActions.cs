using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.ActionsExtractorTests
{
    public class GivenNullActions : WhenTestingExtractActions
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ActionsAreNull)
            .When(ExtractActionsIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnEmptyEnumerable);

        [Given]
        public void ActionsAreNull()
        {
            Actions = null;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnEmptyEnumerable()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Empty);
        }
    }
}