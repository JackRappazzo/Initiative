using System.Text.Json;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpeedExtractorTests
{
    public class GivenComplexSpeedValue : WhenTestingExtractSpeedValue
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SpeedElementIsComplexObject)
            .When(ExtractSpeedValueIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldExtractNumberFromObject);

        [Given]
        public void SpeedElementIsComplexObject()
        {
            var complexSpeed = """{"number": 40, "condition": "(hover)"}""";
            SpeedElement = JsonDocument.Parse(complexSpeed).RootElement;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldExtractNumberFromObject()
        {
            Assert.That(Result, Is.EqualTo(40));
        }
    }
}