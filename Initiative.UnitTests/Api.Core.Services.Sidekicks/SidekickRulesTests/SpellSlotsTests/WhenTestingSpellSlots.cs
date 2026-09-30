using Initiative.Api.Core.Services.Sidekicks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellSlotsTests
{
    public abstract class WhenTestingSpellSlots : ComposableTestingTheBehaviourOf
    {
        protected int Level;
        protected IReadOnlyDictionary<int, int> Slots;

        [When]
        public void SpellSlotsIsCalled()
        {
            Slots = SidekickRules.SpellSlots(Level);
        }
    }
}
