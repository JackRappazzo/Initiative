using System.Reflection.Metadata;
using Initiative.IntegrationTests.Persistence.Utilities;
using Initiative.Persistence.ConnectionStrings;
using LeapingGorilla.Testing.NUnit.Composable;
using MongoDB.Driver;

namespace Initiative.IntegrationTests.Persistence
{
    public abstract class WhenTestingWithMongoDb : ComposableTestingTheBehaviourOf
    {
        protected const string ConnectionString = ConnectionStrings.LocalTest;
    }
}