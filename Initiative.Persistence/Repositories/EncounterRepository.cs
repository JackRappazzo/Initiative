using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using MongoDB.Driver;

namespace Initiative.Persistence.Repositories
{
    public class EncounterRepository : MongoDbRepository
    {
        public const string TableName = "Encounters";

        public EncounterRepository(string connectionString, string databaseName) : base(connectionString, databaseName)
        {

        }

        public async Task<string> CreateEncounter(string ownerId, string name, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Encounter>(TableName);

            var encounter = new Encounter()
            {
                Creatures = [],
                DisplayName = name,
                OwnerId = ownerId
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
    }
}
