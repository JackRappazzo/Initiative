using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.InitiativeCalculatorTests
{
    public class GivenLowDexterity : WhenTestingCalculateInitiativeModifier
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(DexterityIs6AndNoProficiency)
            .When(CalculateInitiativeModifierIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldCalculateNegativeModifier);

        [Given]
        public void DexterityIs6AndNoProficiency()
        {
            Dexterity = 6; // -2 modifier
            InitiativeData = null;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldCalculateNegativeModifier()
        {
            Assert.That(Result, Is.EqualTo(-2)); // (6-10)/2 = -2
        }
    }
}