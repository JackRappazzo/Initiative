using Initiative.Persistence.Configuration;
using Initiative.Persistence.Models.Lobby;
using Initiative.Lobby.Core.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Initiative.Persistence.Repositories
{
    public class LobbyStateRepository : MongoDbRepository, ILobbyStateRepository
    {

        public const string TableName = "LobbyStates";

        public LobbyStateRepository(IDatabaseConnectionFactory factory) : base(factory)
        {
        }

        public async Task<string> UpsertLobbyState(
            string roomCode,
            string[] creatures,
            int turnNumber,
            int currentCreatureIndex,
            LobbyMode currentMode,
            CancellationToken cancellationToken)
        {
            var db = GetMongoDatabase();
            var collection = db.GetCollection<LobbyStateDto>(TableName);

            var lobbyState = new LobbyStateDto
            {
                Id = ObjectId.GenerateNewId().ToString(),
                RoomCode = roomCode,
                Creatures = creatures,
                TurnNumber = turnNumber,
                CurrentCreatureIndex = currentCreatureIndex,
                CurrentMode = currentMode
            };

            var filter = Builders<LobbyStateDto>.Filter.Eq(x => x.RoomCode, roomCode);
            var update = Builders<LobbyStateDto>.Update
                .SetOnInsert(x => x.Id, lobbyState.Id)
                .Set(x => x.Creatures, creatures)
                .Set(x => x.TurnNumber, turnNumber)
                .Set(x => x.CurrentCreatureIndex, currentCreatureIndex)
                .Set(x => x.CurrentMode, currentMode)
                .Set(x => x.LastUpdatedAt, DateTime.UtcNow);

            var result = await collection.UpdateOneAsync(
                filter,
                update,
                new UpdateOptions { IsUpsert = true },
                cancellationToken);

            if (result.UpsertedId != null)
            {
                return result.UpsertedId.ToString();
            }
            else
            {
                var existingLobbyState = await FetchLobbyStateByRoomCode(roomCode, cancellationToken);
                return existingLobbyState.Id;
            }
        }

        public async Task<LobbyStateDto> FetchLobbyStateByRoomCode(string roomCode, CancellationToken cancellationToken)
        {
            var db = GetMongoDatabase();
            var collection = db.GetCollection<LobbyStateDto>("LobbyStates");
            var filter = Builders<LobbyStateDto>.Filter.Eq(x => x.RoomCode, roomCode);
            var lobbyState = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return lobbyState;

        }
    }
}
