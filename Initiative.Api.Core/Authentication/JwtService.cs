using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core;
using Initiative.Api.Core.Identity;
using Initiative.Persistence.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Initiative.Api.Core.Authentication
{
    public class JwtService : IJwtService
    {

        protected IOptions<JwtSettings> jwtSettingsContainer;
        protected ICredentialsFactory credentialsFactory;
        protected IJwtRefreshTokenRepository jwtRefreshTokenRepository;

        public JwtService(IOptions<JwtSettings> settings, ICredentialsFactory securityKeyFactory, IJwtRefreshTokenRepository jwtRefreshTokenRepository)
        {
            this.jwtSettingsContainer = settings;
            this.credentialsFactory = securityKeyFactory;
            this.jwtRefreshTokenRepository = jwtRefreshTokenRepository;

        }

        public static string GetSecret(EnvironmentType environment)
        {
            if (environment == EnvironmentType.Local)
            {
                return Environment.GetEnvironmentVariable("JWT_SECRET");
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public string GenerateToken(InitiativeUser user)
        {

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

            var jwtSettings = jwtSettingsContainer.Value;

            var credentials = credentialsFactory.Create(jwtSettings.Secret);


            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                expires: DateTime.Now + TimeSpan.FromMinutes(jwtSettings.ExpiresInMinutes),
                claims: claims,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<JwtRefreshToken> GenerateAndStoreRefreshToken(InitiativeUser user, DateTime expiration, CancellationToken cancellationToken)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            await jwtRefreshTokenRepository.UpsertRefreshToken(user.Id.ToString(), token, expiration, cancellationToken);

            var returnToken = new JwtRefreshToken()
            {
                User = user,
                Expiration = expiration,
                RefreshToken = token
            };

            return returnToken;

        }
    }
}
