using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.SpellcastingAbilityTests
{
    public class GivenProdigy : WhenTestingSpellcastingAbility
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(KindIsProdigy)
            .When(SpellcastingAbilityIsCalled)
            .Then(ShouldUseCharisma);

        [Given]
        public void KindIsProdigy()
        {
            Kind = SidekickSpellcasterKind.Prodigy;
        }

        [Then]
        public void ShouldUseCharisma()
        {
            Assert.That(Result, Is.EqualTo("cha"));
        }
    }
}
