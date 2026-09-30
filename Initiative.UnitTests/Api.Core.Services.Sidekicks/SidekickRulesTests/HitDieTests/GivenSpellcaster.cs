using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.HitDieTests
{
    public class GivenSpellcaster : WhenTestingHitDie
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ClassIsSpellcaster)
            .When(HitDieIsCalled)
            .Then(ShouldUseD6)
            .And(ShouldAverage4);

        [Given]
        public void ClassIsSpellcaster()
        {
            Class = SidekickClass.Spellcaster;
        }

        [Then]
        public void ShouldUseD6()
        {
            Assert.That(MaxResult, Is.EqualTo(6));
            Assert.That(SizeResult, Is.EqualTo("d6"));
        }

        [Then]
        public void ShouldAverage4()
        {
            Assert.That(AverageResult, Is.EqualTo(4));
        }
    }
}
