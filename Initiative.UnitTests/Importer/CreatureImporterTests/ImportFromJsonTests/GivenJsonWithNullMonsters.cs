using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromJsonTests
{
    public class GivenJsonWithNullMonsters : WhenTestingImportFromJson
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(JsonContentHasNullMonsters)
            .When(ImportFromJsonIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnEmptyList);

        [Given]
        public void JsonContentHasNullMonsters()
        {
            JsonContent = """
            {
                "monster": null
            }
            """;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnEmptyList()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Empty);
        }
    }
}