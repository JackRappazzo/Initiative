namespace Initiative.Api.Core.Services.Authentication
{
    public interface IJwtRefreshService
    {
        Task<(bool refreshTokenValid, string? refreshedJwt)> RefreshJwt(string refreshToken, CancellationToken cancellationToken);
    }
}