using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellProgressionTests
{
    public class GivenLevel20 : WhenTestingSpellProgression
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs20)
            .When(SpellProgressionIsCalled)
            .Then(ShouldKnow4CantripsAnd11Spells);

        [Given]
        public void LevelIs20()
        {
            Level = 20;
        }

        [Then]
        public void ShouldKnow4CantripsAnd11Spells()
        {
            Assert.That(CantripsResult, Is.EqualTo(4));
            Assert.That(SpellsKnownResult, Is.EqualTo(11));
        }
    }
}
