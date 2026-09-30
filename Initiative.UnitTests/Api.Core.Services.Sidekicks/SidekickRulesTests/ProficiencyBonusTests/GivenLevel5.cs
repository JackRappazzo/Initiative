using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.ProficiencyBonusTests
{
    public class GivenLevel5 : WhenTestingProficiencyBonus
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs5)
            .When(ProficiencyBonusIsCalled)
            .Then(ShouldReturn3);

        [Given]
        public void LevelIs5()
        {
            Level = 5;
        }

        [Then]
        public void ShouldReturn3()
        {
            Assert.That(Result, Is.EqualTo(3));
        }
    }
}
