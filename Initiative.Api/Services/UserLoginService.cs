using Initiative.Api.Core.Authentication;
using Initiative.Api.Core.Identity;
using Microsoft.AspNetCore.Identity;

namespace Initiative.Api.Services
{
    public class UserLoginService : IUserLoginService
    {
        UserManager<InitiativeUser> userManager;
        IJwtService jwtService;

        public UserLoginService(UserManager<InitiativeUser> userManager, IJwtService jwtService)
        {
            this.userManager = userManager;
            this.jwtService = jwtService;
        }

        public async Task<(bool success, string message, string token)> LoginAndFetchToken(string email, string password, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = await userManager.FindByNameAsync(email);

                if (user == null)
                {
                    return (false, "No matching email address", null);
                }
                else
                {

                    var passwordMatch = await userManager.CheckPasswordAsync(user, password);
                    if (passwordMatch)
                    {
                        return (true, string.Empty, jwtService.GenerateToken(user));
                    }
                    else
                    {
                        return (false, "Incorrect password", null);
                    }
                }
            }
            else
            {
                return (false, "Missing arguments", null);
            }
        }
    }
}
