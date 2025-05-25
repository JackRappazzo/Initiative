using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.ConnectionStrings;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.EncounterRepositoryTests
{
    public abstract class WhenTestingEncounterRepository : WhenTestingWithMongoDb
    {

        [ItemUnderTest] protected EncounterRepository EncounterRepository;
        [Dependency] protected string DbConnectionString = ConnectionString;
        [Dependency] protected string DatabaseName = Databases.LocalTest;

        protected CancellationToken CancellationToken = default;
    }
}
