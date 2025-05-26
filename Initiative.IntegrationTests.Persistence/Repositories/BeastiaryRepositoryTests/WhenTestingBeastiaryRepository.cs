using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.IntegrationTests.Persistence.Utilities;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.ConnectionStrings;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests
{
    public abstract class WhenTestingBeastiaryRepository : WhenTestingWithMongoDb
    {
        [ItemUnderTest] protected BeastiaryRepository BeastiaryRepository { get; set; }

        [Dependency] public IDatabaseConnectionFactory DatabaseConnectionFactory = new TestConnectionFactory();

        public CancellationToken CancellationToken = default;

    }
}
