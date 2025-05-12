using Initiative.Persistence.Models.Authentication;

namespace Initiative.Persistence.Repositories
{
    public interface IJwtRefreshTokenRepository
    {
        Task<JwtRefreshTokenModel> FetchToken(string refreshToken, CancellationToken cancellationToken);
        Task UpsertRefreshToken(string userId, string refreshToken, DateTime expiration, CancellationToken cancellationToken);
    }
}