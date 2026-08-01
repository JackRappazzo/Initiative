using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Initiative.UnitTests.Api.Controllers.ConditionControllerTests.ResolveConditionTests
{
    public abstract class WhenTestingResolveCondition : WhenTestingConditionController
    {
        protected string QueryName = null!;
        protected IActionResult Result = null!;

        [When]
        public async Task ResolveConditionIsCalled()
        {
            Result = await ConditionController.ResolveCondition(QueryName, CancellationToken);
        }
    }
}
