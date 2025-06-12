using Initiative.Api.Core.Identity;
using Initiative.Api.Core.Utilities;
using Microsoft.AspNetCore.Identity;

namespace Initiative.Api.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {

        UserManager<InitiativeUser> userManager;
        IBase62CodeGenerator codeGenerator;

        public UserRegistrationService(UserManager<InitiativeUser> userManager, IBase62CodeGenerator codeGenerator)
        {
            this.userManager = userManager;
            this.codeGenerator = codeGenerator;
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

            var user = new InitiativeUser() { 
                DisplayName = displayName,
                Email = email, 
                UserName = email, 
                CurrentRoomCode = codeGenerator.GenerateCode(8) 
            };
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return (false, result.Errors.First().Description);
            }
            else
            {
                return (true, string.Empty);
            }
        }
    }
}
