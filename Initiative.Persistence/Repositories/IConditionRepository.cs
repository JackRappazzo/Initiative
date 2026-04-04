using Initiative.Persistence.Models.Condition;

namespace Initiative.Persistence.Repositories
{
    public interface IConditionRepository
    {
        /// <summary>
        /// Inserts or updates conditions.
        /// Matching is performed on Name; existing documents are updated,
        /// new ones are inserted.
        /// </summary>
        Task UpsertConditions(IEnumerable<ConditionDocument> conditions, CancellationToken cancellationToken);

        /// <summary>
        /// Returns all conditions, ordered by Name.
        /// </summary>
        Task<IEnumerable<ConditionDocument>> GetAllConditions(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the full condition document by its ID, or null if not found.
        /// </summary>
        Task<ConditionDocument?> GetConditionById(string conditionId, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a condition by name (case-insensitive match).
        /// </summary>
        Task<ConditionDocument?> GetConditionByName(string name, CancellationToken cancellationToken);
    }
}
