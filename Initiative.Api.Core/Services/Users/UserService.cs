using Initiative.Api.Core.Identity;
using Initiative.Api.Core.Services.Users;
using Initiative.Api.Core.Utilities;
using Initiative.Persistence.Models;
using Initiative.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Initiative.Api.Core.Services.Users
{
    public class UserService : IUserService
    {

        IUserManager<ApplicationIdentity> userManager;
        IInitiativeUserRepository userRepository;
        IBase62CodeGenerator codeGenerator;

        public UserService(IUserManager<ApplicationIdentity> userManager, IInitiativeUserRepository userRepository, IBase62CodeGenerator codeGenerator)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
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
            var emailIsInUse = await IsEmailInUse(email, cancellationToken);
            if (emailIsInUse)
            {
                return (false, "Email exists");
            }

            var (identityCreated, userId) = await CreateIdentity(displayName, email, password, cancellationToken);

            if (!identityCreated)
            {
                return (false, "Failed to create identity");
            }
            else
            {
                try
                {
                    await CreateInitiativeUser(userId, email, cancellationToken);
                    return (true, string.Empty);
                }
                catch (Exception ex)
                {
                    // If we fail to create the user, we need to delete the user from Identity
                    await DeleteIdentity(userId, cancellationToken);
                    return (false, ex.Message);
                }
            }
        }

        private async Task<(bool, string)> CreateIdentity(string displayName, string email, string password, CancellationToken cancellationToken)
        {
            var user = new Core.Identity.ApplicationIdentity()
            {
                Email = email,
                UserName = email
            };
            var result = await userManager.CreateAsync(user, password);
            return (result.Succeeded, user.Id.ToString());
        }

        private async Task<bool> DeleteIdentity(string userId, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var result = await userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        private async Task CreateInitiativeUser(string identityId, string email, CancellationToken cancellationToken)
        {
            var user = new InitiativeUserModel()
            {
                IdentityId = identityId,
                CreatedAt = DateTime.UtcNow,
                EmailAddress = email,
                RoomCode = codeGenerator.GenerateCode(6)
            };

            var existingUser = await userRepository.FetchUserByIdentityId(identityId, cancellationToken);
            if(existingUser != null)
            {
                // User already exists, no need to create a new one
                return;
            }
            var userId = await userRepository.InsertUser(user, cancellationToken);
        }

        private async Task<bool> IsEmailInUse(string email, CancellationToken cancellationToken)
        {
            return (await userManager.FindByEmailAsync(email)) != null;
        }
    }
}
