using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Initiative.Api.Core;
using Initiative.Api.Core.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Initiative.Api.Core.Authentication
{
    public class JwtService
    {

        public JwtService() { }

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

        public string GenerateToken(InitiativeUser user, JwtSettings settings)
        {

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                expires: DateTime.Now + TimeSpan.FromMinutes(settings.ExpiresInMinutes),
                claims: claims,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
