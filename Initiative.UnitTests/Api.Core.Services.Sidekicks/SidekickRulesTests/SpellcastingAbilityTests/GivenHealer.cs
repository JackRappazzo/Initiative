using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellcastingAbilityTests
{
    public class GivenHealer : WhenTestingSpellcastingAbility
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(KindIsHealer)
            .When(SpellcastingAbilityIsCalled)
            .Then(ShouldUseWisdom);

        [Given]
        public void KindIsHealer()
        {
            Kind = SidekickSpellcasterKind.Healer;
        }

        [Then]
        public void ShouldUseWisdom()
        {
            Assert.That(Result, Is.EqualTo("wis"));
        }
    }
}
