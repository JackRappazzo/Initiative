using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromJsonTests
{
    public class GivenJsonWithNoMonsters : WhenTestingImportFromJson
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(JsonContentHasNoMonsters)
            .When(ImportFromJsonIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnEmptyList);

        [Given]
        public void JsonContentHasNoMonsters()
        {
            JsonContent = """
            {
                "monster": []
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