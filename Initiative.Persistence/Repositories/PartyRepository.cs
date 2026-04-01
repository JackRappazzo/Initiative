using Initiative.Persistence.Configuration;
using Initiative.Persistence.Models.Parties;
using MongoDB.Driver;

namespace Initiative.Persistence.Repositories
{
    public class PartyRepository : MongoDbRepository, IPartyRepository
    {
        public const string TableName = "Parties";

        public PartyRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<string> CreateParty(string ownerId, string name, IEnumerable<PartyMember> members, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<PartyDocument>(TableName);

            var party = new PartyDocument
            {
                Name = name,
                OwnerId = ownerId,
                Members = members ?? []
            };

            await collection.InsertOneAsync(party, new InsertOneOptions { BypassDocumentValidation = false }, cancellationToken);

            return party.Id;
        }

        public async Task<PartyDocument?> FetchPartyById(string partyId, string ownerId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<PartyDocument>(TableName);
            var result = await collection.FindAsync(p => p.Id == partyId && p.OwnerId == ownerId, cancellationToken: cancellationToken);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<PartyDocument>> FetchPartiesByOwnerId(string ownerId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<PartyDocument>(TableName);
            var filter = Builders<PartyDocument>.Filter.Eq(p => p.OwnerId, ownerId);
            return await collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<bool> DeleteParty(string partyId, string ownerId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<PartyDocument>(TableName);
            var result = await collection.DeleteOneAsync(p => p.Id == partyId && p.OwnerId == ownerId, cancellationToken: cancellationToken);
            return result.DeletedCount > 0;
        }
    }
}
