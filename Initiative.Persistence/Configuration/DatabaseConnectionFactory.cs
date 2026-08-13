using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Constants;
using Microsoft.Extensions.Configuration;


namespace Initiative.Persistence.Configuration
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly IConfiguration configuration;

        public DatabaseConnectionFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public DatabaseConnectionConfiguration Create()
        {
            var connectionString =
                Environment.GetEnvironmentVariable(EnvironmentVariableNames.MongoDbConnectionString) ??
                configuration.GetConnectionString("MongoDb") ??
                configuration["MongoDb:ConnectionString"] ??
                ConnectionStrings.Local;

            return new DatabaseConnectionConfiguration()
            {
                ConnectionString = connectionString,
                DatabaseName = Databases.Main
            };
        }
    }
}
