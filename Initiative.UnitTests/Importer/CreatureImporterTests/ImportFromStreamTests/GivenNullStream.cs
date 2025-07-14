using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromStreamTests
{
    public class GivenNullStream : WhenTestingImportFromStream
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(StreamIsNull)
            .When(ImportFromStreamAsyncIsCalled)
            .Then(ShouldThrowArgumentNullException);

        [Given]
        public void StreamIsNull()
        {
            Stream = null;
        }

        [Then]
        public void ShouldThrowArgumentNullException()
        {
            Assert.That(ThrownException, Is.Not.Null);
            Assert.That(ThrownException, Is.InstanceOf<ArgumentNullException>());
            Assert.That(ThrownException.Message, Does.Contain("stream"));
        }
    }
}