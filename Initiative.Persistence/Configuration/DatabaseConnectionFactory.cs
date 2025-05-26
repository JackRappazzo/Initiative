using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Persistence.Configuration
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        public DatabaseConnectionConfiguration Create()
        {
            return new DatabaseConnectionConfiguration()
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "initiative"
            };
        }
    }
}
