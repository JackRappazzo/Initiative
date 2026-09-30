using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellProgressionTests
{
    public class GivenLevel4 : WhenTestingSpellProgression
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs4)
            .When(SpellProgressionIsCalled)
            .Then(ShouldKnow3CantripsAnd3Spells);

        [Given]
        public void LevelIs4()
        {
            Level = 4;
        }

        [Then]
        public void ShouldKnow3CantripsAnd3Spells()
        {
            Assert.That(CantripsResult, Is.EqualTo(3));
            Assert.That(SpellsKnownResult, Is.EqualTo(3));
        }
    }
}
