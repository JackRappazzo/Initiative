using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Configuration;
using MongoDB.Driver;

namespace Initiative.Persistence
{
    public abstract class MongoDbRepository
    {
        string connectionString;
        string databaseName;


        protected MongoDbRepository(IDatabaseConnectionFactory factory)
        {
            var configuration = factory.Create();

            this.connectionString = configuration.ConnectionString;
            this.databaseName = configuration.DatabaseName;
        }

        protected IMongoDatabase GetMongoDatabase()
        {
            return new MongoClient(connectionString).GetDatabase(databaseName);

        }
    }
}
