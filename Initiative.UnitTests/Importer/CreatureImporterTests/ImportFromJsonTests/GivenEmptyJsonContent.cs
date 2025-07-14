using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromJsonTests
{
    public class GivenEmptyJsonContent : WhenTestingImportFromJson
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(JsonContentIsEmpty)
            .When(ImportFromJsonIsCalled)
            .Then(ShouldThrowArgumentException);

        [Given]
        public void JsonContentIsEmpty()
        {
            JsonContent = "";
        }

        [Then]
        public void ShouldThrowArgumentException()
        {
            Assert.That(ThrownException, Is.Not.Null);
            Assert.That(ThrownException, Is.InstanceOf<ArgumentException>());
            Assert.That(ThrownException.Message, Does.Contain("JSON content cannot be null or empty"));
        }
    }
}