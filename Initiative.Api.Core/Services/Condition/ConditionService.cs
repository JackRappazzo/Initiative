using Initiative.Persistence.Models.Condition;
using Initiative.Persistence.Repositories;

namespace Initiative.Api.Core.Services.Condition
{
    public class ConditionService : IConditionService
    {
        private readonly IConditionRepository _repository;

        public ConditionService(IConditionRepository repository)
        {
            _repository = repository;
        }

        public Task<ConditionDocument?> GetConditionByName(string name, CancellationToken cancellationToken)
            => _repository.GetConditionByName(name, cancellationToken);
    }
}
