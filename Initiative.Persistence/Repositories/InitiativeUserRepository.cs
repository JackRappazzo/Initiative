using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Models;
using Initiative.Persistence.Extensions;
using MongoDB.Driver;

namespace Initiative.Persistence.Repositories
{
    public class InitiativeUserRepository : MongoDbRepository, IInitiativeUserRepository
    {
        public const string TableName = "InitiativeUsers";

        public InitiativeUserRepository(IDatabaseConnectionFactory factory) : base(factory)
        {

        }

        public async Task<string> InsertUser(InitiativeUserModel user, CancellationToken cancellationToken)
        {
            if(!user.IdentityId.IsValidObjectId())
            {
                throw new ArgumentException("IdentityId cannot be null or empty", nameof(user.IdentityId));
            }

            if(await UserExists(user.IdentityId, cancellationToken))
            {
                throw new InvalidOperationException($"User with IdentityId {user.IdentityId} already exists.");
            }


            var collection = GetMongoDatabase().GetCollection<InitiativeUserModel>(TableName);
            await collection.InsertOneAsync(user, new InsertOneOptions() { BypassDocumentValidation = false }, cancellationToken);
            return user.Id;
        }

        public async Task<InitiativeUserModel> FetchUserByIdentityId(string identityId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<InitiativeUserModel>(TableName);
            var user = await collection.FindAsync(u => u.IdentityId == identityId, cancellationToken: cancellationToken);
            return user.FirstOrDefault();
        }

        public async Task<bool> UserExists(string identityId, CancellationToken cancellationToken)
        {
            var collection = GetMongoDatabase().GetCollection<InitiativeUserModel>(TableName);
            var count = await collection.CountDocumentsAsync(u => u.IdentityId == identityId, cancellationToken: cancellationToken);
            return count > 0;
        }

    }
}
