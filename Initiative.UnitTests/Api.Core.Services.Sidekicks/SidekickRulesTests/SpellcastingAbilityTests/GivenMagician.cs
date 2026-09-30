using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellcastingAbilityTests
{
    public class GivenMagician : WhenTestingSpellcastingAbility
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(KindIsMagician)
            .When(SpellcastingAbilityIsCalled)
            .Then(ShouldUseIntelligence);

        [Given]
        public void KindIsMagician()
        {
            Kind = SidekickSpellcasterKind.Magician;
        }

        [Then]
        public void ShouldUseIntelligence()
        {
            Assert.That(Result, Is.EqualTo("int"));
        }
    }
}
