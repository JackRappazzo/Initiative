using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.InitiativeCalculatorTests
{
    public abstract class WhenTestingCalculateInitiativeModifier : WhenTestingInitiativeCalculator
    {
        protected int Dexterity;
        protected InitiativeJson? InitiativeData;
        protected int Result;

        [When(DoNotRethrowExceptions: true)]
        public void CalculateInitiativeModifierIsCalled()
        {
            Result = InitiativeCalculator.CalculateInitiativeModifier(Dexterity, InitiativeData);
        }
    }
}