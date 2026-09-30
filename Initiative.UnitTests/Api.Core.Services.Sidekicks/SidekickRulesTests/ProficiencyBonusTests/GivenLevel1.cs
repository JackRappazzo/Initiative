using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.ProficiencyBonusTests
{
    public class GivenLevel1 : WhenTestingProficiencyBonus
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs1)
            .When(ProficiencyBonusIsCalled)
            .Then(ShouldReturn2);

        [Given]
        public void LevelIs1()
        {
            Level = 1;
        }

        [Then]
        public void ShouldReturn2()
        {
            Assert.That(Result, Is.EqualTo(2));
        }
    }
}
