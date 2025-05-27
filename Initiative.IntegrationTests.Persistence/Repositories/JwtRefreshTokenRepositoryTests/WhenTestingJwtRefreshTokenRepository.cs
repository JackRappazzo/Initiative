using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.IntegrationTests.Persistence.Utilities;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Constants;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.JwtRefreshTokenRepositoryTests
{
    public abstract class WhenTestingJwtRefreshTokenRepository : WhenTestingWithMongoDb
    {
        [ItemUnderTest]
        public JwtRefreshTokenRepository JwtRefreshTokenRepository;

        [Dependency]
        public IDatabaseConnectionFactory DatabaseConnectionFactory = new TestConnectionFactory();

        public CancellationToken CancellationToken = default;
    }
}
