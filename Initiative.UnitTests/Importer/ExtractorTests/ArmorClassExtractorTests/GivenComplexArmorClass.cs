using System.Text.Json;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.ArmorClassExtractorTests
{
    public class GivenComplexArmorClass : WhenTestingExtractArmorClass
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ArmorClassIsComplexObject)
            .When(ExtractArmorClassIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldExtractArmorClassFromObject);

        [Given]
        public void ArmorClassIsComplexObject()
        {
            var acJson = """{"ac": 18, "from": ["plate armor"]}""";
            ArmorClassList = new List<JsonElement> { JsonDocument.Parse(acJson).RootElement };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldExtractArmorClassFromObject()
        {
            Assert.That(Result, Is.EqualTo(18));
        }
    }
}