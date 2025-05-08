using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Initiative.Persistence
{
    public abstract class MongoDbRepository
    {
        string connectionString;
        string databaseName;


        protected MongoDbRepository(string connectionString, string databaseName)
        {
            this.connectionString = connectionString;
            this.databaseName = databaseName;
           
        }

        protected IMongoDatabase GetMongoDatabase()
        {
            return new MongoClient(connectionString).GetDatabase(databaseName);

        }
    }
}
