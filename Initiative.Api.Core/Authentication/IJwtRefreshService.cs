
namespace Initiative.Api.Core.Authentication
{
    public interface IJwtRefreshService
    {
        Task<(bool refreshTokenValid, string? refreshedJwt)> RefreshJwt(string refreshToken, CancellationToken cancellationToken);
    }
}