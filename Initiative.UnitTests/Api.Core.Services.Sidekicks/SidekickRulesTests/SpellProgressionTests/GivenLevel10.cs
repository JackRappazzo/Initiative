using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellProgressionTests
{
    public class GivenLevel10 : WhenTestingSpellProgression
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs10)
            .When(SpellProgressionIsCalled)
            .Then(ShouldKnow4CantripsAnd6Spells);

        [Given]
        public void LevelIs10()
        {
            Level = 10;
        }

        [Then]
        public void ShouldKnow4CantripsAnd6Spells()
        {
            Assert.That(CantripsResult, Is.EqualTo(4));
            Assert.That(SpellsKnownResult, Is.EqualTo(6));
        }
    }
}
