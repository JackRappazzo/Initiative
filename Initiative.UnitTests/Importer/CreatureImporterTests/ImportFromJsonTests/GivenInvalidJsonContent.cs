using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromJsonTests
{
    public class GivenInvalidJsonContent : WhenTestingImportFromJson
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(JsonContentIsInvalid)
            .When(ImportFromJsonIsCalled)
            .Then(ShouldThrowInvalidOperationException);

        [Given]
        public void JsonContentIsInvalid()
        {
            JsonContent = "{ invalid json content }";
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