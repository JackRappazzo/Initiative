using Initiative.Persistence.Models.Condition;

namespace Initiative.Api.Core.Services.Condition
{
    public interface IConditionService
    {
        Task<ConditionDocument?> GetConditionByName(string name, CancellationToken cancellationToken);
    }
}
