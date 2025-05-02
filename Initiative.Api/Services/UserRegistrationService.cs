using Initiative.Api.Core.Identity;
using Microsoft.AspNetCore.Identity;

namespace Initiative.Api.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {

        UserManager<InitiativeUser> userManager;

        public UserRegistrationService(UserManager<InitiativeUser> userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<(bool, string)> RegisterUser(string displayName, string email, string password, CancellationToken cancellationToken)
        {
            if (await userManager.FindByEmailAsync(email) != null)
                return (false, "Email exists");

            var user = new InitiativeUser() { DisplayName = displayName, EmailAddress = email };
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return (false, "An error occurred");
            }
            else
            {
                return (true, string.Empty);
            }
        }
    }
}
