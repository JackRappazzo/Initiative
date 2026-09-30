using Initiative.Api.Core.Services.Sidekicks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.AbilityScoreIncreaseLevelsTests
{
    public class GivenDefault : ComposableTestingTheBehaviourOf
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NoSetupRequired)
            .When(AbilityScoreIncreaseLevelsIsRead)
            .Then(ShouldMatchTasha);

        [Given]
        public void NoSetupRequired()
        {
        }

        [When]
        public void AbilityScoreIncreaseLevelsIsRead()
        {
        }

        [Then]
        public void ShouldMatchTasha()
        {
            Assert.That(SidekickRules.AbilityScoreIncreaseLevels, Is.EquivalentTo(new[] { 4, 8, 12, 16, 19 }));
        }
    }
}
