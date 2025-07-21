using System.Text.Json;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpeedExtractorTests
{
    public class GivenSimpleSpeedValue : WhenTestingExtractSpeedValue
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SpeedElementIsSimpleNumber)
            .When(ExtractSpeedValueIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnCorrectSpeed);

        [Given]
        public void SpeedElementIsSimpleNumber()
        {
            SpeedElement = JsonDocument.Parse("30").RootElement;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnCorrectSpeed()
        {
            Assert.That(Result, Is.EqualTo(30));
        }
    }
}