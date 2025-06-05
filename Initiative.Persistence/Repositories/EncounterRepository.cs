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

        /// <summary>
        /// Creates a new encounter for the specified user with the given name.
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Fetches an encounter by its ID and owner ID.
        /// </summary>
        /// <param name="encounterId"></param>
        /// <param name="ownerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Encounter> FetchEncounterById(string encounterId, string ownerId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Encounter>(TableName);

            var encounter = await collection.FindAsync(e => e.Id == encounterId && e.OwnerId == ownerId, cancellationToken: cancellationToken);

            return encounter.First();
        }

        /// <summary>
        /// Fetches a list of encounters for a specific user by their user ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        public async Task SetEncounterCreatures(string encounterId, IEnumerable<Creature> creatures, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Encounter>(TableName);
            var update = Builders<Encounter>.Update.Set(e => e.Creatures, creatures);
            await collection.UpdateOneAsync(e => e.Id == encounterId, update, cancellationToken: cancellationToken);
        }

        public async Task SetEncounterName(string encounterId, string newName, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Encounter>(TableName);
            var update = Builders<Encounter>.Update.Set(e => e.DisplayName, newName);
            await collection.UpdateOneAsync(e => e.Id == encounterId, update, cancellationToken: cancellationToken);

        }

        public async Task<bool> DoesUserOwnEncounter(string encounterId, string ownerId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Encounter>(TableName);
            var filter = Builders<Encounter>.Filter.And(
                Builders<Encounter>.Filter.Eq(e => e.Id, encounterId),
                Builders<Encounter>.Filter.Eq(e => e.OwnerId, ownerId)
            );
            var count = await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            return count > 0;
        }

        public async Task<bool> DeleteEncounter(string encounterId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<Encounter>(TableName);
            var result = await collection.DeleteOneAsync(e => e.Id == encounterId, cancellationToken: cancellationToken);
            return result.DeletedCount > 0;
        }
    }
}
