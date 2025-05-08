using System.Reflection.Metadata;
using Initiative.IntegrationTests.Persistence.Utilities;
using Initiative.Persistence.ConnectionStrings;
using LeapingGorilla.Testing.NUnit.Composable;
using MongoDB.Driver;

namespace Initiative.IntegrationTests.Persistence
{
    public abstract class WhenTestingWithMongoDb : ComposableTestingTheBehaviourOf
    {
        private MongoTestResetService mongoTestResetService = new MongoTestResetService(ConnectionStrings.LocalTest);
        protected const string ConnectionString = ConnectionStrings.LocalTest;
        protected const string DatabaseName = "Initiative_Test";

        public async Task ResetDatabase()
        {
            await mongoTestResetService.ResetDatabase();
        }

        protected IMongoDatabase GetDatabase()
        {
            return new MongoClient(ConnectionString).GetDatabase(DatabaseName);
        }

    }
}