using Microsoft.IdentityModel.Tokens;

namespace Initiative.Api.Core.Authentication
{
    public interface ICredentialsFactory
    {
        SigningCredentials Create(string secret);
    }
}