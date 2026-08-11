using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using Initiative.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace Initiative.Api.Core.Services.Authentication
{
    public class JwtRefreshService : IJwtRefreshService
    {
        protected IJwtService jwtService;
        protected IJwtRefreshTokenRepository jwtRefreshTokenRepository;
        protected IUserManager<ApplicationIdentity> userManager;
        protected IOptions<JwtSettings> jwtSettingsContainer;

        public JwtRefreshService(IJwtService service, IJwtRefreshTokenRepository jwtRepository, IUserManager<ApplicationIdentity> userManager, IOptions<JwtSettings> jwtSettings)
        {
            jwtRefreshTokenRepository = jwtRepository;
            jwtService = service;
            this.userManager = userManager;
            jwtSettingsContainer = jwtSettings;
        }

        public async Task<(bool refreshTokenValid, string? refreshedJwt)> RefreshJwt(string refreshToken, CancellationToken cancellationToken)
        {

            var token = await jwtRefreshTokenRepository.FetchToken(refreshToken, cancellationToken);

            if (token == null || token.Expiration < DateTime.Now)
            {
                return (false, null);
            }
            else
            {
                var user = await userManager.FindByIdAsync(token.UserId.ToString());
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                var newExpiration = DateTime.UtcNow.AddDays(jwtSettingsContainer.Value.RefreshTokenExpiresInDays);
                await jwtRefreshTokenRepository.UpsertRefreshToken(token.UserId.ToString(), refreshToken, newExpiration, cancellationToken);

                var jwt = jwtService.GenerateToken(user);
                return (true, jwt);
            }
        }
    }
}
