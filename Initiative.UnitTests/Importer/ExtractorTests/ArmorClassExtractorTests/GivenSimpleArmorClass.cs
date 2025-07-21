using System.Text.Json;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.ArmorClassExtractorTests
{
    public class GivenSimpleArmorClass : WhenTestingExtractArmorClass
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ArmorClassIsSimpleNumber)
            .When(ExtractArmorClassIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnCorrectArmorClass);

        [Given]
        public void ArmorClassIsSimpleNumber()
        {
            ArmorClassList = new List<JsonElement> { JsonDocument.Parse("15").RootElement };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnCorrectArmorClass()
        {
            Assert.That(Result, Is.EqualTo(15));
        }
    }
}