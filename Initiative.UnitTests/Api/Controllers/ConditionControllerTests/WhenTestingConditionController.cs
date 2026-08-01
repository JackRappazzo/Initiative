using Initiative.Api.Controllers;
using Initiative.Api.Core.Services.Condition;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Controllers.ConditionControllerTests
{
    public abstract class WhenTestingConditionController : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected ConditionController ConditionController = null!;

        [Dependency]
        protected IConditionService ConditionService = null!;

        protected CancellationToken CancellationToken = default;
    }
}
