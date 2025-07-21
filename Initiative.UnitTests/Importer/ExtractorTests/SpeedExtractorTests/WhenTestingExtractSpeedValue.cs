using System.Text.Json;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpeedExtractorTests
{
    public abstract class WhenTestingExtractSpeedValue : WhenTestingSpeedExtractor
    {
        protected JsonElement? SpeedElement;
        protected int? Result;

        [When(DoNotRethrowExceptions: true)]
        public void ExtractSpeedValueIsCalled()
        {
            Result = SpeedExtractor.ExtractSpeedValue(SpeedElement);
        }
    }
}