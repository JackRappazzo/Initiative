using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.InitiativeCalculatorTests
{
    public class GivenBasicDexterity : WhenTestingCalculateInitiativeModifier
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(DexterityIs14AndNoProficiency)
            .When(CalculateInitiativeModifierIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldCalculateCorrectModifier);

        [Given]
        public void DexterityIs14AndNoProficiency()
        {
            Dexterity = 14; // +2 modifier
            InitiativeData = null;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldCalculateCorrectModifier()
        {
            Assert.That(Result, Is.EqualTo(2)); // (14-10)/2 = 2
        }
    }
}