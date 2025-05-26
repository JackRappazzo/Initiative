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
    public class BeastiaryRepository : MongoDbRepository
    {
        public const string TableName = "Beastiaries";

        public BeastiaryRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        public async Task<string> CreateUserBeastiary(string? ownerId, string name, CancellationToken cancellationToken)
        {
            var db = GetMongoDatabase();
            var collection = db.GetCollection<Beastiary>(TableName);

            var beastiary = new Beastiary()
            {
                Name = name,
                OwnerId = ownerId,
                Creatures = []
            };


            await collection.InsertOneAsync(beastiary, new InsertOneOptions() { BypassDocumentValidation = false }, cancellationToken);
            return beastiary.Id;
        }

        public async Task<string> CreateSystemBeastiary(string name, CancellationToken cancellationToken) =>
            await CreateUserBeastiary(null, name, cancellationToken);

        public async Task<Beastiary> GetUserBeastiary(string beastiaryId, string ownerId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Beastiary>(TableName);

            var result = await collection.FindAsync<Beastiary>(b => b.Id == beastiaryId && b.OwnerId == ownerId, cancellationToken: cancellationToken);
            return result.First();
        }

        public async Task<Beastiary> GetSystemBeastiary(string beastiaryId, CancellationToken cancellationToken)
        {
            return await GetUserBeastiary(beastiaryId, null, cancellationToken: cancellationToken);
        }

        public async Task<string> AddCreature(string beastiaryId, Creature creature, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Beastiary>(TableName);
            creature.Id = ObjectId.GenerateNewId().ToString();

            var update = Builders<Beastiary>.Update.Push(b => b.Creatures, creature);
            var result = await collection.UpdateOneAsync(b => b.Id == beastiaryId, update, cancellationToken: cancellationToken);

            if (result.ModifiedCount > 0)
            {
                return creature.Id;
            }
            else return null;
        }
    }
}
