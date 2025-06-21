using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;

namespace Initiative.Api.Core.Services.Authentication
{
    public class JwtRefreshToken
    {
        public ApplicationIdentity User { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
