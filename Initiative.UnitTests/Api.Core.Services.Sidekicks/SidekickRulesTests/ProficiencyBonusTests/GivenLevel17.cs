using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.ProficiencyBonusTests
{
    public class GivenLevel17 : WhenTestingProficiencyBonus
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs17)
            .When(ProficiencyBonusIsCalled)
            .Then(ShouldReturn6);

        [Given]
        public void LevelIs17()
        {
            Level = 17;
        }

        [Then]
        public void ShouldReturn6()
        {
            Assert.That(Result, Is.EqualTo(6));
        }
    }
}
