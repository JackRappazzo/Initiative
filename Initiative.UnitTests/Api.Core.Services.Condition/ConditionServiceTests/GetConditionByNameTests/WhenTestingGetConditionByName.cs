using Initiative.Persistence.Models.Condition;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Condition.ConditionServiceTests.GetConditionByNameTests
{
    public abstract class WhenTestingGetConditionByName : WhenTestingConditionService
    {
        protected string Name = null!;
        protected ConditionDocument? Result;

        [When]
        public async Task GetConditionByNameIsCalled()
        {
            Result = await ConditionService.GetConditionByName(Name, CancellationToken);
        }
    }
}
