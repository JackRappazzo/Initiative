using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Models.Encounters;
using Initiative.Persistence.Models.Encounters.Dtos;
using MongoDB.Driver;

namespace Initiative.Persistence.Repositories
{
    public class EncounterRepository : MongoDbRepository, IEncounterRepository
    {
        public const string TableName = "Encounters";

        public EncounterRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        public async Task<string> CreateEncounter(string ownerId, string name, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Encounter>(TableName);

            var encounter = new Encounter()
            {
                Creatures = [],
                DisplayName = name,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            await collection.InsertOneAsync(encounter, new InsertOneOptions() { BypassDocumentValidation = false }, cancellationToken);

            return encounter.Id;
        }

        public async Task<Encounter> FetchEncounterById(string encounterId, string ownerId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Encounter>(TableName);

            var encounter = await collection.FindAsync(e => e.Id == encounterId && e.OwnerId == ownerId, cancellationToken: cancellationToken);

            return encounter.First();
        }

        public async Task<IEnumerable<EncounterListItemDto>> FetchEncounterListByUserId(string userId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Encounter>(TableName);

            var filter = Builders<Encounter>.Filter.Eq(e => e.OwnerId, userId);

            var results = await collection.Find(filter)
                .Project(e =>
                    new EncounterListItemDto()
                    {
                        EncounterId = e.Id,
                        EncounterName = e.DisplayName,
                        NumberOfCreatures = e.Creatures.Count(),
                        CreatedAt = e.CreatedAt
                    }
                )
                .ToListAsync(cancellationToken);

            return results;

        }
    }
}
