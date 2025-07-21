using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.InitiativeCalculatorTests
{
    public class GivenDexterityWithProficiency : WhenTestingCalculateInitiativeModifier
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(DexterityIs16WithProficiency)
            .When(CalculateInitiativeModifierIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldAddProficiencyToModifier);

        [Given]
        public void DexterityIs16WithProficiency()
        {
            Dexterity = 16; // +3 modifier
            InitiativeData = new InitiativeJson { Proficiency = 2 };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldAddProficiencyToModifier()
        {
            Assert.That(Result, Is.EqualTo(5)); // (16-10)/2 + 2 = 5
        }
    }
}