using Initiative.Api.Core.Identity;

namespace Initiative.Api.Core.Authentication
{
    public interface IJwtService
    {
        Task<JwtRefreshToken> GenerateAndStoreRefreshToken(InitiativeUser user, DateTime expiration, CancellationToken cancellationToken);
        
        string GenerateToken(InitiativeUser user);
    }
}