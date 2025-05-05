using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Initiative.Api.Core;
using Initiative.Api.Core.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Initiative.Api.Core.Authentication
{
    public class JwtService : IJwtService
    {

        protected IOptions<JwtSettings> jwtSettingsContainer;
        protected ICredentialsFactory credentialsFactory;

        public JwtService(IOptions<JwtSettings> settings, ICredentialsFactory securityKeyFactory)
        {
            this.jwtSettingsContainer = settings;
            this.credentialsFactory = securityKeyFactory;
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
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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
    }
}
