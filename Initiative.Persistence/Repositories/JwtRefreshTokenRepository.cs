using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Models.Authentication;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Initiative.Persistence.Repositories
{
    public class JwtRefreshTokenRepository : MongoDbRepository, IJwtRefreshTokenRepository
    {
        public const string TableName = "UserRefreshTokens";

        public JwtRefreshTokenRepository(IDatabaseConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task UpsertRefreshToken(string userId, string refreshToken, DateTime expiration, CancellationToken cancellationToken)
        {
            var database = GetMongoDatabase();
            var collection = database.GetCollection<JwtRefreshTokenModel>(TableName);

            var filter = Builders<JwtRefreshTokenModel>.Filter.Eq(refreshToken => refreshToken.UserId, new ObjectId(userId));

            var currentToken = await collection.Find<JwtRefreshTokenModel>(filter).FirstOrDefaultAsync();


            await collection.ReplaceOneAsync<JwtRefreshTokenModel>(
                token => token.UserId == new ObjectId(userId),
                new JwtRefreshTokenModel()
                {
                    Id = currentToken?.Id ?? new ObjectId(),
                    UserId = new ObjectId(userId),
                    RefreshToken = refreshToken,
                    Expiration = expiration
                },
                new ReplaceOptions()
                {
                    IsUpsert = true
                },
                cancellationToken);

        }

        public async Task<JwtRefreshTokenModel> FetchToken(string refreshToken, CancellationToken cancellationToken)
        {
            var database = GetMongoDatabase();
            var collection = database.GetCollection<JwtRefreshTokenModel>(TableName);

            var filter = Builders<JwtRefreshTokenModel>.Filter.Eq(r => r.RefreshToken, refreshToken);

            var token = await (await collection.FindAsync<JwtRefreshTokenModel>(filter, cancellationToken: cancellationToken)).FirstOrDefaultAsync();

            return token;
        }
    }
}
