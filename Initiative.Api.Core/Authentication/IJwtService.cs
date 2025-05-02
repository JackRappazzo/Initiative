using Initiative.Api.Core.Identity;

namespace Initiative.Api.Core.Authentication
{
    public interface IJwtService
    {
        static abstract string GetSecret(EnvironmentType environment);
        string GenerateToken(InitiativeUser user);
    }
}