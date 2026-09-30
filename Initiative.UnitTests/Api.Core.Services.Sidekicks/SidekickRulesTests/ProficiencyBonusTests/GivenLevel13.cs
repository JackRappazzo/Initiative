using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.ProficiencyBonusTests
{
    public class GivenLevel13 : WhenTestingProficiencyBonus
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs13)
            .When(ProficiencyBonusIsCalled)
            .Then(ShouldReturn5);

        [Given]
        public void LevelIs13()
        {
            Level = 13;
        }

        [Then]
        public void ShouldReturn5()
        {
            Assert.That(Result, Is.EqualTo(5));
        }
    }
}
