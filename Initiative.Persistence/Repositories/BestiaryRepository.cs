using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Models.Encounters;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Initiative.Persistence.Repositories
{
    public class BestiaryRepository : MongoDbRepository, IBestiaryRepository
    {
        public const string TableName = "Bestiaries";

        public BestiaryRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        public async Task<string> CreateUserBestiary(string? ownerId, string name, CancellationToken cancellationToken)
        {
            var db = GetMongoDatabase();
            var collection = db.GetCollection<Bestiary>(TableName);

            var beastiary = new Bestiary()
            {
                Name = name,
                OwnerId = ownerId,
                Creatures = []
            };


            await collection.InsertOneAsync(beastiary, new InsertOneOptions() { BypassDocumentValidation = false }, cancellationToken);
            return beastiary.Id;
        }

        public async Task<string> CreateSystemBestiary(string name, CancellationToken cancellationToken) =>
            await CreateUserBestiary(null, name, cancellationToken);

        public async Task<Bestiary> GetUserBestiary(string bestiaryId, string ownerId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Bestiary>(TableName);

            var result = await collection.FindAsync<Bestiary>(b => b.Id == bestiaryId && b.OwnerId == ownerId, cancellationToken: cancellationToken);
            return result.First();
        }

        public async Task<Bestiary> GetSystemBestiary(string beastiaryId, CancellationToken cancellationToken)
        {
            return await GetUserBestiary(beastiaryId, null, cancellationToken: cancellationToken);
        }

        public async Task<Bestiary> GetSystemBestiaryByName(string name, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Bestiary>(TableName);
            var result = await collection.FindAsync(b => b.Name == name && b.OwnerId == null, cancellationToken: cancellationToken);
            return result.FirstOrDefault();
        }

        public async Task<string> UpsertSystemBestiary(string name, IEnumerable<Creature> creatures, CancellationToken cancellationToken)
        {
            var bestiary = await GetSystemBestiaryByName(name, cancellationToken);
            if (bestiary == null)
            {
                var bestiaryId = await CreateSystemBestiary(name, cancellationToken);
                bestiary = await GetSystemBestiary(bestiaryId, cancellationToken);
            }

            // Update existing bestiary
            var collection = GetMongoDatabase().GetCollection<Bestiary>(TableName);
            var update = Builders<Bestiary>.Update.Set(b => b.Creatures, creatures.ToList());
            await collection.UpdateOneAsync(b => b.Id == bestiary.Id, update, cancellationToken: cancellationToken);
            return bestiary.Id;
        }

        public async Task<string> AddCreature(string bestiaryId, Creature creature, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Bestiary>(TableName);
            creature.Id = ObjectId.GenerateNewId().ToString();

            var update = Builders<Bestiary>.Update.Push(b => b.Creatures, creature);
            var result = await collection.UpdateOneAsync(b => b.Id == bestiaryId, update, cancellationToken: cancellationToken);

            if (result.ModifiedCount > 0)
            {
                return creature.Id;
            }
            else
            { 
                return null; 
            }
        }
    }
}
