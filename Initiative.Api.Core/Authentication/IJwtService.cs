using Initiative.Api.Core.Identity;

namespace Initiative.Api.Core.Authentication
{
    public interface IJwtService
    {
        JwtRefreshToken GenerateRefreshToken(InitiativeUser user, DateTime expiration);
        string GenerateToken(InitiativeUser user);
    }
}