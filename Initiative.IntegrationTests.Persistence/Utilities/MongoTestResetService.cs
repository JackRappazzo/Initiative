﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Repositories;
using MongoDB.Driver;

namespace Initiative.IntegrationTests.Persistence.Utilities
{
    public class MongoTestResetService
    {
        MongoClient client;
        public MongoTestResetService(string connectionString)
        {
            client = new MongoClient(connectionString);
            
        }

        public async Task ResetDatabase()
        {
            var database = client.GetDatabase("Initiative_Test");
            await ResetCollection(database, JwtRefreshTokenRepository.TableName);
            await ResetCollection(database, InitiativeUserRepository.TableName);
            await ResetCollection(database, EncounterRepository.TableName);
            await ResetCollection(database, BestiaryRepository.TableName);
            await ResetCollection(database, LobbyStateRepository.TableName);
        }

        protected async Task ResetCollection(IMongoDatabase database, string collectionName)
        {
            if((await database.ListCollectionNames().ToListAsync()).Contains(collectionName))
            {
                await database.DropCollectionAsync(collectionName);
            }
            await database.CreateCollectionAsync(collectionName);
        }
    }
}
