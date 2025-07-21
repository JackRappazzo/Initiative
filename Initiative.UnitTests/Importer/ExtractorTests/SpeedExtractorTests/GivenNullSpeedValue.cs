using System.Text.Json;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpeedExtractorTests
{
    public class GivenNullSpeedValue : WhenTestingExtractSpeedValue
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SpeedElementIsNull)
            .When(ExtractSpeedValueIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnNull);

        [Given]
        public void SpeedElementIsNull()
        {
            SpeedElement = null;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnNull()
        {
            Assert.That(Result, Is.Null);
        }
    }
}