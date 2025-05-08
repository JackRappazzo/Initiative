using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.ConnectionStrings;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.JwtRefreshTokenRepositoryTests
{
    public abstract class WhenTestingJwtRefreshTokenRepository : WhenTestingWithMongoDb
    {
        [ItemUnderTest]
        public JwtRefreshTokenRepository JwtRefreshTokenRepository;

        [Dependency]
        public string DbConnectionString = ConnectionString;

        [Dependency]
        public string DatabaseName = Databases.LocalTest;
        
        public CancellationToken CancellationToken = default;
    }
}
