using Initiative.Persistence.Configuration;
using Initiative.Persistence.Models.Spell;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Initiative.Persistence.Repositories
{
    public class SpellRepository : MongoDbRepository, ISpellRepository
    {
        public const string SpellSourcesCollection = "SpellSources";
        public const string SpellsCollection = "Spells";

        public SpellRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
            EnsureIndexes();
        }

        public async Task<string> CreateSpellSource(SpellSourceDocument spellSource, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<SpellSourceDocument>(SpellSourcesCollection);
            await collection.InsertOneAsync(spellSource, cancellationToken: cancellationToken);
            return spellSource.Id;
        }

        public async Task<SpellSourceDocument?> GetSpellSourceBySource(string source, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<SpellSourceDocument>(SpellSourcesCollection);
            var result = await collection.FindAsync(
                s => s.Source == source,
                cancellationToken: cancellationToken);
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<SpellSourceDocument>> GetAllSpellSources(CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<SpellSourceDocument>(SpellSourcesCollection);
            return await collection.Find(FilterDefinition<SpellSourceDocument>.Empty)
                .SortBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task UpsertSpells(IEnumerable<SpellDocument> spells, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<SpellDocument>(SpellsCollection);

            var writes = spells.Select(spell =>
            {
                var filter = Builders<SpellDocument>.Filter.And(
                    Builders<SpellDocument>.Filter.Eq(s => s.SpellSourceId, spell.SpellSourceId),
                    Builders<SpellDocument>.Filter.Eq(s => s.Name, spell.Name)
                );

                var update = Builders<SpellDocument>.Update
                    .Set(s => s.Source, spell.Source)
                    .Set(s => s.School, spell.School)
                    .Set(s => s.RawData, spell.RawData)
                    .SetOnInsert(s => s.SpellSourceId, spell.SpellSourceId)
                    .SetOnInsert(s => s.Name, spell.Name);

                return new UpdateOneModel<SpellDocument>(filter, update)
                {
                    IsUpsert = true
                };
            }).ToList();

            if (writes.Count > 0)
            {
                await collection.BulkWriteAsync(writes, new BulkWriteOptions { IsOrdered = false }, cancellationToken);
            }
        }

        public async Task<IEnumerable<SpellDocument>> SearchSpells(SpellSearchQuery query, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<SpellDocument>(SpellsCollection);
            var filter = BuildSpellFilter(query);

            return await collection.Find(filter)
                .SortBy(s => s.Name)
                .Skip(query.Skip)
                .Limit(query.PageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> CountSpells(SpellSearchQuery query, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<SpellDocument>(SpellsCollection);
            var filter = BuildSpellFilter(query);
            return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<SpellDocument?> GetSpellById(string spellId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<SpellDocument>(SpellsCollection);
            var result = await collection.FindAsync(
                s => s.Id == spellId,
                cancellationToken: cancellationToken);
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<SpellDocument?> GetSpellByNameAndSource(string name, string? source, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<SpellDocument>(SpellsCollection);

            var filters = new List<FilterDefinition<SpellDocument>>();

            var nameRegex = new BsonRegularExpression(
                $"^{System.Text.RegularExpressions.Regex.Escape(name.Trim())}$", "i");
            filters.Add(Builders<SpellDocument>.Filter.Regex(s => s.Name, nameRegex));

            if (!string.IsNullOrWhiteSpace(source))
            {
                filters.Add(Builders<SpellDocument>.Filter.Eq(s => s.Source, source.Trim()));
            }

            var filter = Builders<SpellDocument>.Filter.And(filters);
            var result = await collection.FindAsync(filter, cancellationToken: cancellationToken);
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        private static FilterDefinition<SpellDocument> BuildSpellFilter(SpellSearchQuery query)
        {
            var filters = new List<FilterDefinition<SpellDocument>>();

            var sourceIds = query.SpellSourceIds?.ToList();
            if (sourceIds is { Count: > 0 })
            {
                var validIds = sourceIds.Where(id => ObjectId.TryParse(id, out _)).ToList();
                filters.Add(Builders<SpellDocument>.Filter.In(s => s.SpellSourceId, validIds));
            }

            if (!string.IsNullOrWhiteSpace(query.NameSearch))
            {
                var escaped = System.Text.RegularExpressions.Regex.Escape(query.NameSearch.Trim());
                var regex = new BsonRegularExpression(escaped, "i");
                filters.Add(Builders<SpellDocument>.Filter.Regex(s => s.Name, regex));
            }

            if (!string.IsNullOrWhiteSpace(query.School))
            {
                var regex = new BsonRegularExpression(
                    $"^{System.Text.RegularExpressions.Regex.Escape(query.School.Trim())}$", "i");
                filters.Add(Builders<SpellDocument>.Filter.Regex(s => s.School, regex));
            }

            return filters.Count > 0
                ? Builders<SpellDocument>.Filter.And(filters)
                : FilterDefinition<SpellDocument>.Empty;
        }

        private void EnsureIndexes()
        {
            var db = GetMongoDatabase();

            var spellCollection = db.GetCollection<SpellDocument>(SpellsCollection);

            // Compound index on (SpellSourceId, School) for common filter combinations
            var compoundIndex = Builders<SpellDocument>.IndexKeys
                .Ascending(s => s.SpellSourceId)
                .Ascending(s => s.School);
            spellCollection.Indexes.CreateOne(new CreateIndexModel<SpellDocument>(compoundIndex));

            // Index on Name for sort and prefix queries
            var nameIndex = Builders<SpellDocument>.IndexKeys.Ascending(s => s.Name);
            spellCollection.Indexes.CreateOne(new CreateIndexModel<SpellDocument>(nameIndex));

            // Unique constraint on (SpellSourceId, Name) to support upsert logic
            var uniqueIndex = Builders<SpellDocument>.IndexKeys
                .Ascending(s => s.SpellSourceId)
                .Ascending(s => s.Name);
            spellCollection.Indexes.CreateOne(new CreateIndexModel<SpellDocument>(
                uniqueIndex,
                new CreateIndexOptions { Unique = true }));

            // SpellSources: sparse unique index on Source
            var sourceCollection = db.GetCollection<SpellSourceDocument>(SpellSourcesCollection);
            var sourceIndex = Builders<SpellSourceDocument>.IndexKeys.Ascending(s => s.Source);
            sourceCollection.Indexes.CreateOne(new CreateIndexModel<SpellSourceDocument>(
                sourceIndex,
                new CreateIndexOptions { Unique = true, Sparse = true }));
        }
    }
}
