using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellSlotsTests
{
    public class GivenFirstLevel : WhenTestingSpellSlots
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs1)
            .When(SpellSlotsIsCalled)
            .Then(ShouldOnlyHaveFirstLevelSlots);

        [Given]
        public void LevelIs1()
        {
            Level = 1;
        }

        [Then]
        public void ShouldOnlyHaveFirstLevelSlots()
        {
            Assert.That(Slots[1], Is.EqualTo(2));
            Assert.That(Slots.ContainsKey(2), Is.False);
        }
    }
}
