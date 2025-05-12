using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using Initiative.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Serializers;

namespace Initiative.Api.Core.Authentication
{
    public class JwtRefreshService
    {
        protected IJwtService jwtService;
        protected IJwtRefreshTokenRepository jwtRefreshTokenRepository;
        protected UserManager<InitiativeUser> userManager;

        public JwtRefreshService(IJwtService service, IJwtRefreshTokenRepository jwtRepository, UserManager<InitiativeUser> userManager)
        {
            this.jwtRefreshTokenRepository = jwtRepository;
            this.jwtService = service;
            this.userManager = userManager;
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

                var jwt = jwtService.GenerateToken(user);
                return (true, jwt);
            }
        }
    }
}
