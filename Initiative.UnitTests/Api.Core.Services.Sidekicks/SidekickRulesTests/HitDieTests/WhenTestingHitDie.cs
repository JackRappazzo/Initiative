using Initiative.Api.Core.Services.Sidekicks;
using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickRulesTests.HitDieTests
{
    public abstract class WhenTestingHitDie : ComposableTestingTheBehaviourOf
    {
        protected SidekickClass Class;
        protected int MaxResult;
        protected string SizeResult;
        protected int AverageResult;

        [When]
        public void HitDieIsCalled()
        {
            MaxResult = SidekickRules.HitDieMax(Class);
            SizeResult = SidekickRules.HitDieSize(Class);
            AverageResult = SidekickRules.HitDieAverage(Class);
        }
    }
}
