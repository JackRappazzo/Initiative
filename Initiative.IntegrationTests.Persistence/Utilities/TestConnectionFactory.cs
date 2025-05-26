using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.ConnectionStrings;

namespace Initiative.IntegrationTests.Persistence.Utilities
{
    public class TestConnectionFactory : IDatabaseConnectionFactory
    {
        public DatabaseConnectionConfiguration Create()
        {
            return new DatabaseConnectionConfiguration()
            {
                ConnectionString = ConnectionStrings.LocalTest,
                DatabaseName = Databases.LocalTest
            };
        }
    }
}
