using Initiative.IntegrationTests.Persistence.Utilities;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.PartyRepositoryTests
{
    public abstract class WhenTestingPartyRepository : WhenTestingWithMongoDb
    {
        [ItemUnderTest] protected PartyRepository PartyRepository;
        [Dependency] public IDatabaseConnectionFactory DatabaseConnectionFactory = new TestConnectionFactory();

        protected CancellationToken CancellationToken = default;
    }
}
