using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.ProficiencyBonusTests
{
    public class GivenLevel9 : WhenTestingProficiencyBonus
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(LevelIs9)
            .When(ProficiencyBonusIsCalled)
            .Then(ShouldReturn4);

        [Given]
        public void LevelIs9()
        {
            Level = 9;
        }

        [Then]
        public void ShouldReturn4()
        {
            Assert.That(Result, Is.EqualTo(4));
        }
    }
}
