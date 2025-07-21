using System.Text.Json;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.ArmorClassExtractorTests
{
    public class GivenNullArmorClass : WhenTestingExtractArmorClass
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ArmorClassIsNull)
            .When(ExtractArmorClassIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnDefaultArmorClass);

        [Given]
        public void ArmorClassIsNull()
        {
            ArmorClassList = null;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnDefaultArmorClass()
        {
            Assert.That(Result, Is.EqualTo(10));
        }
    }
}