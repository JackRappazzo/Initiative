using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.HitDieTests
{
    public class GivenExpert : WhenTestingHitDie
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ClassIsExpert)
            .When(HitDieIsCalled)
            .Then(ShouldUseD8)
            .And(ShouldAverage5);

        [Given]
        public void ClassIsExpert()
        {
            Class = SidekickClass.Expert;
        }

        [Then]
        public void ShouldUseD8()
        {
            Assert.That(MaxResult, Is.EqualTo(8));
            Assert.That(SizeResult, Is.EqualTo("d8"));
        }

        [Then]
        public void ShouldAverage5()
        {
            Assert.That(AverageResult, Is.EqualTo(5));
        }
    }
}
