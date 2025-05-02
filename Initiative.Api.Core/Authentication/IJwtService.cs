using Initiative.Api.Core.Identity;

namespace Initiative.Api.Core.Authentication
{
    public interface IJwtService
    {
        string GenerateToken(InitiativeUser user);
    }
}