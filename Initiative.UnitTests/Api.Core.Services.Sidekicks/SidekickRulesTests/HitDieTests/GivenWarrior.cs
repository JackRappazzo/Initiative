using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.HitDieTests
{
    public class GivenWarrior : WhenTestingHitDie
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ClassIsWarrior)
            .When(HitDieIsCalled)
            .Then(ShouldUseD10)
            .And(ShouldAverage6);

        [Given]
        public void ClassIsWarrior()
        {
            Class = SidekickClass.Warrior;
        }

        [Then]
        public void ShouldUseD10()
        {
            Assert.That(MaxResult, Is.EqualTo(10));
            Assert.That(SizeResult, Is.EqualTo("d10"));
        }

        [Then]
        public void ShouldAverage6()
        {
            Assert.That(AverageResult, Is.EqualTo(6));
        }
    }
}
