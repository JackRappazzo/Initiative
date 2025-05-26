using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.IntegrationTests.Persistence.Utilities;
using Initiative.Persistence.ConnectionStrings;

namespace Initiative.IntegrationTests
{
    [SetUpFixture]
    public class SetupFixture
    {
        private MongoTestResetService mongoTestResetService = new MongoTestResetService(ConnectionStrings.LocalTest);
        protected const string ConnectionString = ConnectionStrings.LocalTest;
        protected const string DatabaseName = Databases.LocalTest;

        [OneTimeSetUp]
        public async Task ResetDatabase()
        {
            await mongoTestResetService.ResetDatabase();
        }

    }
}
