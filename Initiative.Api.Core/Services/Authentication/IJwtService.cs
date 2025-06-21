using Initiative.Api.Core.Identity;

namespace Initiative.Api.Core.Services.Authentication
{
    public interface IJwtService
    {
        Task<JwtRefreshToken> GenerateAndStoreRefreshToken(ApplicationIdentity user, DateTime expiration, CancellationToken cancellationToken);
        
        string GenerateToken(ApplicationIdentity user);
    }
}