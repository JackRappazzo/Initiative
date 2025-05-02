using Initiative.Api.Core.Identity;
using Microsoft.AspNetCore.Identity;

namespace Initiative.Api.Services
{
    public class UserLoginService : IUserLoginService
    {
        UserManager<InitiativeUser> userManager;

        public UserLoginService(UserManager<InitiativeUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<(bool success, string message)> Login(string email, string password, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = await userManager.FindByNameAsync(email);

                if (user == null)
                {
                    return (false, "No matching email address");
                }

                var passwordMatch = await userManager.CheckPasswordAsync(user, password);
                if (passwordMatch)
                {
                    return (true, string.Empty);
                }
                else
                {
                    return (false, "Incorrect password");
                }
            }
            else
            {
                return (false, "Missing arguments");
            }
        }
    }
}
