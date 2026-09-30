using Initiative.Api.Core.Services.Sidekicks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.ProficiencyBonusTests
{
    public abstract class WhenTestingProficiencyBonus : ComposableTestingTheBehaviourOf
    {
        protected int Level;
        protected int Result;

        [When]
        public void ProficiencyBonusIsCalled()
        {
            Result = SidekickRules.ProficiencyBonus(Level);
        }
    }
}
