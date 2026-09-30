using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellProgressionTests
{
    public class GivenLevel1 : WhenTestingSpellProgression
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs1)
            .When(SpellProgressionIsCalled)
            .Then(ShouldKnow2CantripsAnd1Spell);

        [Given]
        public void LevelIs1()
        {
            Level = 1;
        }

        [Then]
        public void ShouldKnow2CantripsAnd1Spell()
        {
            Assert.That(CantripsResult, Is.EqualTo(2));
            Assert.That(SpellsKnownResult, Is.EqualTo(1));
        }
    }
}
