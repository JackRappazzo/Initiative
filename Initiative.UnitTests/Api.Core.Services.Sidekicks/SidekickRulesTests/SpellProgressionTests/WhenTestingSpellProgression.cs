using Initiative.Api.Core.Services.Sidekicks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellProgressionTests
{
    public abstract class WhenTestingSpellProgression : ComposableTestingTheBehaviourOf
    {
        protected int Level;
        protected int CantripsResult;
        protected int SpellsKnownResult;

        [When]
        public void SpellProgressionIsCalled()
        {
            CantripsResult = SidekickRules.CantripsKnown(Level);
            SpellsKnownResult = SidekickRules.SpellsKnown(Level);
        }
    }
}
