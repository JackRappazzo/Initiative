using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Constants;


namespace Initiative.Persistence.Configuration
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        public DatabaseConnectionFactory()
        {
        }

        public DatabaseConnectionConfiguration Create()
        {
            var connectionString = Environment.GetEnvironmentVariable(EnvironmentVariableNames.MongoDbConnectionString) ?? ConnectionStrings.Local;

            return new DatabaseConnectionConfiguration()
            {
                ConnectionString = connectionString,
                DatabaseName = Databases.Main
            };
        }
    }
}
