using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellSlotsTests
{
    public class GivenTwentiethLevel : WhenTestingSpellSlots
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs20)
            .When(SpellSlotsIsCalled)
            .Then(ShouldCapAtFifthLevel);

        [Given]
        public void LevelIs20()
        {
            Level = 20;
        }

        [Then]
        public void ShouldCapAtFifthLevel()
        {
            Assert.That(Slots.Keys.Max(), Is.EqualTo(5));
            Assert.That(Slots[5], Is.EqualTo(3));
        }
    }
}
