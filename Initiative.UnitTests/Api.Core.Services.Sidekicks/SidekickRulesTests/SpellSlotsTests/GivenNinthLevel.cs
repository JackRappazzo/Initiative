using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellSlotsTests
{
    public class GivenNinthLevel : WhenTestingSpellSlots
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs9)
            .When(SpellSlotsIsCalled)
            .Then(ShouldIncludeFifthLevelSlots);

        [Given]
        public void LevelIs9()
        {
            Level = 9;
        }

        [Then]
        public void ShouldIncludeFifthLevelSlots()
        {
            Assert.That(Slots[1], Is.EqualTo(4));
            Assert.That(Slots[2], Is.EqualTo(3));
            Assert.That(Slots[3], Is.EqualTo(3));
            Assert.That(Slots[4], Is.EqualTo(3));
            Assert.That(Slots[5], Is.EqualTo(1));
        }
    }
}
