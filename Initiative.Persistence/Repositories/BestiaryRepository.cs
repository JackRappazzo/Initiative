using Initiative.Persistence.Configuration;
using Initiative.Persistence.Models.Bestiary;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Initiative.Persistence.Repositories
{
    public class BestiaryRepository : MongoDbRepository, IBestiaryRepository
    {
        public const string BestiariesCollection = "Bestiaries";
        public const string CreaturesCollection = "BestiaryCreatures";

        public BestiaryRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
            EnsureIndexes();
        }

        public async Task<string> CreateBestiary(BestiaryDocument bestiary, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<BestiaryDocument>(BestiariesCollection);
            await collection.InsertOneAsync(bestiary, cancellationToken: cancellationToken);
            return bestiary.Id;
        }

        public async Task<BestiaryDocument?> GetBestiaryBySource(string source, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<BestiaryDocument>(BestiariesCollection);
            var result = await collection.FindAsync(
                b => b.Source == source,
                cancellationToken: cancellationToken);
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<BestiaryDocument>> GetAllBestiaries(CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<BestiaryDocument>(BestiariesCollection);
            return await collection.Find(FilterDefinition<BestiaryDocument>.Empty)
                .SortBy(b => b.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BestiaryDocument>> GetBestariesByOwner(string ownerId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<BestiaryDocument>(BestiariesCollection);
            return await collection.Find(b => b.OwnerId == ownerId)
                .SortBy(b => b.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task UpsertCreatures(IEnumerable<BestiaryCreatureDocument> creatures, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<BestiaryCreatureDocument>(CreaturesCollection);

            var writes = creatures.Select(creature =>
            {
                var filter = Builders<BestiaryCreatureDocument>.Filter.And(
                    Builders<BestiaryCreatureDocument>.Filter.Eq(c => c.BestiaryId, creature.BestiaryId),
                    Builders<BestiaryCreatureDocument>.Filter.Eq(c => c.Name, creature.Name)
                );

                // Use $set rather than a full replace so the immutable _id field is never altered
                var update = Builders<BestiaryCreatureDocument>.Update
                    .Set(c => c.Source, creature.Source)
                    .Set(c => c.CreatureType, creature.CreatureType)
                    .Set(c => c.ChallengeRating, creature.ChallengeRating)
                    .Set(c => c.IsLegendary, creature.IsLegendary)
                    .Set(c => c.RawData, creature.RawData)
                    .SetOnInsert(c => c.BestiaryId, creature.BestiaryId)
                    .SetOnInsert(c => c.Name, creature.Name);

                return new UpdateOneModel<BestiaryCreatureDocument>(filter, update)
                {
                    IsUpsert = true
                };
            }).ToList();

            if (writes.Count > 0)
            {
                await collection.BulkWriteAsync(writes, new BulkWriteOptions { IsOrdered = false }, cancellationToken);
            }
        }

        public async Task<IEnumerable<BestiaryCreatureDocument>> SearchCreatures(BestiarySearchQuery query, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<BestiaryCreatureDocument>(CreaturesCollection);
            var combinedFilter = BuildCreatureFilter(query);

            if (query.SortBy == CreatureSortBy.ChallengeRating)
            {
                return await SearchCreaturesSortedByCr(collection, combinedFilter, query, cancellationToken);
            }

            var findFluent = collection.Find(combinedFilter);

            if (query.SortBy == CreatureSortBy.Type)
            {
                findFluent = query.SortDescending
                    ? findFluent.SortByDescending(c => c.CreatureType).ThenBy(c => c.Name)
                    : findFluent.SortBy(c => c.CreatureType).ThenBy(c => c.Name);
            }
            else
            {
                findFluent = query.SortDescending
                    ? findFluent.SortByDescending(c => c.Name)
                    : findFluent.SortBy(c => c.Name);
            }

            return await findFluent
                .Skip(query.Skip)
                .Limit(query.PageSize)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sorts by CR numerically (e.g. "1/4" → 0.25) using an aggregation pipeline,
        /// then by Name as a tie-breaker. Required because CR is stored as a string.
        /// </summary>
        private static async Task<IEnumerable<BestiaryCreatureDocument>> SearchCreaturesSortedByCr(
            IMongoCollection<BestiaryCreatureDocument> collection,
            FilterDefinition<BestiaryCreatureDocument> filter,
            BestiarySearchQuery query,
            CancellationToken cancellationToken)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", filter.Render(new RenderArgs<BestiaryCreatureDocument>(collection.DocumentSerializer, BsonSerializer.SerializerRegistry))),
                new BsonDocument("$addFields", new BsonDocument("_crSort", new BsonDocument("$switch", new BsonDocument
                {
                    { "branches", new BsonArray
                        {
                            CrBranch("0",    0),
                            CrBranch("1/8",  0.125),
                            CrBranch("1/4",  0.25),
                            CrBranch("1/2",  0.5),
                        }
                        .AddRange(Enumerable.Range(1, 30).Select(i => CrBranch(i.ToString(), i)))
                    },
                    { "default", -1 }
                }))),
                new BsonDocument("$sort", new BsonDocument { { "_crSort", query.SortDescending ? -1 : 1 }, { "Name", 1 } }),
                new BsonDocument("$skip",  query.Skip),
                new BsonDocument("$limit", query.PageSize),
                new BsonDocument("$unset", "_crSort")
            };

            return await collection
                .Aggregate<BestiaryCreatureDocument>(pipeline, cancellationToken: cancellationToken)
                .ToListAsync(cancellationToken);
        }

        private static BsonDocument CrBranch(string crValue, double numericValue)
            => new BsonDocument
            {
                { "case", new BsonDocument("$eq", new BsonArray { "$ChallengeRating", crValue }) },
                { "then", numericValue }
            };

        public async Task<long> CountCreatures(BestiarySearchQuery query, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<BestiaryCreatureDocument>(CreaturesCollection);
            var combinedFilter = BuildCreatureFilter(query);
            return await collection.CountDocumentsAsync(combinedFilter, cancellationToken: cancellationToken);
        }

        private static FilterDefinition<BestiaryCreatureDocument> BuildCreatureFilter(BestiarySearchQuery query)
        {
            var filters = new List<FilterDefinition<BestiaryCreatureDocument>>();

            // Bestiary filter
            var bestiaryIds = query.BestiaryIds?.ToList();
            if (bestiaryIds is { Count: > 0 })
            {
                var objectIds = bestiaryIds
                    .Where(id => ObjectId.TryParse(id, out _))
                    .Select(id => id)
                    .ToList();

                filters.Add(Builders<BestiaryCreatureDocument>.Filter.In(c => c.BestiaryId, objectIds));
            }

            // Name filter: case-insensitive substring match
            if (!string.IsNullOrWhiteSpace(query.NameSearch))
            {
                var escapedName = System.Text.RegularExpressions.Regex.Escape(query.NameSearch.Trim());
                var regex = new BsonRegularExpression(escapedName, "i");
                filters.Add(Builders<BestiaryCreatureDocument>.Filter.Regex(c => c.Name, regex));
            }

            // Creature type: case-insensitive exact match
            if (!string.IsNullOrWhiteSpace(query.CreatureType))
            {
                var regex = new BsonRegularExpression($"^{System.Text.RegularExpressions.Regex.Escape(query.CreatureType.Trim())}$", "i");
                filters.Add(Builders<BestiaryCreatureDocument>.Filter.Regex(c => c.CreatureType, regex));
            }

            // Challenge rating: exact string match (e.g. "1/4", "5")
            if (!string.IsNullOrWhiteSpace(query.ChallengeRating))
            {
                filters.Add(Builders<BestiaryCreatureDocument>.Filter.Eq(c => c.ChallengeRating, query.ChallengeRating.Trim()));
            }

            // Legendary filter
            if (query.IsLegendary.HasValue)
            {
                filters.Add(Builders<BestiaryCreatureDocument>.Filter.Eq(c => c.IsLegendary, query.IsLegendary.Value));
            }

            return filters.Count > 0
                ? Builders<BestiaryCreatureDocument>.Filter.And(filters)
                : FilterDefinition<BestiaryCreatureDocument>.Empty;
        }

        public async Task<BestiaryCreatureDocument?> GetCreatureById(string creatureId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<BestiaryCreatureDocument>(CreaturesCollection);
            var result = await collection.FindAsync(
                c => c.Id == creatureId,
                cancellationToken: cancellationToken);
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        private void EnsureIndexes()
        {
            var db = GetMongoDatabase();

            // BestiaryCreatures: compound index for common filter combinations
            var creatureCollection = db.GetCollection<BestiaryCreatureDocument>(CreaturesCollection);
            var compoundIndex = Builders<BestiaryCreatureDocument>.IndexKeys
                .Ascending(c => c.BestiaryId)
                .Ascending(c => c.CreatureType)
                .Ascending(c => c.ChallengeRating)
                .Ascending(c => c.IsLegendary);
            creatureCollection.Indexes.CreateOne(new CreateIndexModel<BestiaryCreatureDocument>(compoundIndex));

            // BestiaryCreatures: index on Name for sort and prefix queries
            var nameIndex = Builders<BestiaryCreatureDocument>.IndexKeys.Ascending(c => c.Name);
            creatureCollection.Indexes.CreateOne(new CreateIndexModel<BestiaryCreatureDocument>(nameIndex));

            // BestiaryCreatures: unique constraint on (BestiaryId, Name) to support upsert logic
            var uniqueIndex = Builders<BestiaryCreatureDocument>.IndexKeys
                .Ascending(c => c.BestiaryId)
                .Ascending(c => c.Name);
            creatureCollection.Indexes.CreateOne(new CreateIndexModel<BestiaryCreatureDocument>(
                uniqueIndex,
                new CreateIndexOptions { Unique = true }));

            // Bestiaries: sparse unique index on Source.
            // Uniqueness is enforced only for non-null Source values (system bestiaries).
            // Null Source (custom/user bestiaries) is excluded from the uniqueness constraint.
            var bestiaryCollection = db.GetCollection<BestiaryDocument>(BestiariesCollection);
            var sourceIndex = Builders<BestiaryDocument>.IndexKeys.Ascending(b => b.Source);
            bestiaryCollection.Indexes.CreateOne(new CreateIndexModel<BestiaryDocument>(
                sourceIndex,
                new CreateIndexOptions { Unique = true, Sparse = true }));
        }
    }
}
