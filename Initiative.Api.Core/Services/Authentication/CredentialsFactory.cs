using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Initiative.Api.Core.Services.Authentication
{
    public class CredentialsFactory : ICredentialsFactory
    {

        public CredentialsFactory() { }

        public SigningCredentials Create(string secret)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
    }
}
