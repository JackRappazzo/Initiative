using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromStreamTests
{
    public class GivenStreamWithInvalidJson : WhenTestingImportFromStream
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(StreamContainsInvalidJson)
            .When(ImportFromStreamAsyncIsCalled)
            .Then(ShouldThrowInvalidOperationException);

        [Given]
        public void StreamContainsInvalidJson()
        {
            var invalidJsonContent = "{ invalid json content }";
            var bytes = Encoding.UTF8.GetBytes(invalidJsonContent);
            Stream = new MemoryStream(bytes);
        }

        [Then]
        public void ShouldThrowInvalidOperationException()
        {
            Assert.That(ThrownException, Is.Not.Null);
            Assert.That(ThrownException, Is.InstanceOf<InvalidOperationException>());
            Assert.That(ThrownException.Message, Does.Contain("Failed to parse JSON content"));
        }
    }
}