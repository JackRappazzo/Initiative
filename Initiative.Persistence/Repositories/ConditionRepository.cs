using Initiative.Persistence.Configuration;
using Initiative.Persistence.Models.Condition;
using MongoDB.Driver;

namespace Initiative.Persistence.Repositories
{
    public class ConditionRepository : MongoDbRepository, IConditionRepository
    {
        public const string ConditionsCollection = "ConditionsAndStatuses";

        public ConditionRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
            EnsureIndexes();
        }

        public async Task UpsertConditions(IEnumerable<ConditionDocument> conditions, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<ConditionDocument>(ConditionsCollection);

            var writes = conditions.Select(condition =>
            {
                var filter = Builders<ConditionDocument>.Filter.And(
                    Builders<ConditionDocument>.Filter.Eq(c => c.Name, condition.Name),
                    Builders<ConditionDocument>.Filter.Eq(c => c.Type, condition.Type)
                );

                var update = Builders<ConditionDocument>.Update
                    .Set(c => c.Source, condition.Source)
                    .Set(c => c.Type, condition.Type)
                    .Set(c => c.RawData, condition.RawData)
                    .SetOnInsert(c => c.Name, condition.Name);

                return new UpdateOneModel<ConditionDocument>(filter, update) { IsUpsert = true };
            }).ToList();

            if (writes.Count > 0)
            {
                await collection.BulkWriteAsync(writes, cancellationToken: cancellationToken);
            }
        }

        public async Task<IEnumerable<ConditionDocument>> GetAllConditions(CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<ConditionDocument>(ConditionsCollection);
            return await collection.Find(FilterDefinition<ConditionDocument>.Empty)
                .SortBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<ConditionDocument?> GetConditionById(string conditionId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<ConditionDocument>(ConditionsCollection);
            var result = await collection.FindAsync(
                c => c.Id == conditionId,
                cancellationToken: cancellationToken);
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ConditionDocument?> GetConditionByName(string name, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<ConditionDocument>(ConditionsCollection);
            var result = await collection.FindAsync(
                c => c.Name.ToLower() == name.ToLower(),
                cancellationToken: cancellationToken);
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        private void EnsureIndexes()
        {
            var collection = GetMongoDatabase().GetCollection<ConditionDocument>(ConditionsCollection);

            var indexKeysDefinition = Builders<ConditionDocument>.IndexKeys.Ascending(c => c.Name);
            try
            {
                collection.Indexes.CreateOne(new CreateIndexModel<ConditionDocument>(indexKeysDefinition));
            }
            catch
            {
                // Index may already exist
            }
        }
    }
}
