using Initiative.Api.Core.Services.Sidekicks;
using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellcastingAbilityTests
{
    public abstract class WhenTestingSpellcastingAbility : ComposableTestingTheBehaviourOf
    {
        protected SidekickSpellcasterKind Kind;
        protected string Result;

        [When]
        public void SpellcastingAbilityIsCalled()
        {
            Result = SidekickRules.SpellcastingAbility(Kind);
        }
    }
}
