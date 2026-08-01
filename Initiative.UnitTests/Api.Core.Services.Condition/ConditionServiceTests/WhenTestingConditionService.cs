using Initiative.Api.Core.Services.Condition;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Core.Services.Condition.ConditionServiceTests
{
    public abstract class WhenTestingConditionService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected ConditionService ConditionService = null!;

        [Dependency]
        protected IConditionRepository ConditionRepository = null!;

        protected CancellationToken CancellationToken = default;
    }
}
