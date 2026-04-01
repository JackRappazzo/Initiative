using System.Reflection.Metadata;
using Initiative.IntegrationTests.Persistence.Utilities;
using Initiative.Persistence.Constants;
using LeapingGorilla.Testing.NUnit.Composable;
using MongoDB.Driver;

namespace Initiative.IntegrationTests.Persistence
{
    public abstract class WhenTestingWithMongoDb : ComposableTestingTheBehaviourOf
    {
        protected const string ConnectionString = ConnectionStrings.LocalTest;
        protected readonly MongoTestResetService mongoTestResetService = new MongoTestResetService(ConnectionStrings.LocalTest);

        public override async Task SetupAsync()
        {
            await mongoTestResetService.ResetDatabase();
            await base.SetupAsync();
        }
    }
}