using System;
using System.Threading;
using Initiative.IntegrationTests.Persistence.Utilities;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.LobbyStateRepositoryTests
{
    public abstract class WhenTestingLobbyStateRepository : WhenTestingWithMongoDb
    {
        [ItemUnderTest] 
        protected LobbyStateRepository LobbyStateRepository;
        
        [Dependency] 
        public IDatabaseConnectionFactory DatabaseConnectionFactory = new TestConnectionFactory();

        protected CancellationToken CancellationToken = default;
    }
}