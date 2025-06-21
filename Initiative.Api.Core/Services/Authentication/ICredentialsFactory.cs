using Microsoft.IdentityModel.Tokens;

namespace Initiative.Api.Core.Services.Authentication
{
    public interface ICredentialsFactory
    {
        SigningCredentials Create(string secret);
    }
}