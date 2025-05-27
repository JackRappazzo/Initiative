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
            return new DatabaseConnectionConfiguration()
            {
                ConnectionString = Constants.ConnectionStrings.Local,
                DatabaseName = Databases.Local
            };
        }
    }
}
