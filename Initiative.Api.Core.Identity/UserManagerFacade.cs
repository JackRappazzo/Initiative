using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Initiative.Api.Core.Identity
{
    public class UserManagerFacade<TUser> : IUserManager<TUser> where TUser : ApplicationIdentity
    {
        private UserManager<TUser> userManager;

        public UserManagerFacade(UserManager<TUser> userManager)
        {
            this.userManager = userManager;
        }

        public Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            return userManager.CheckPasswordAsync(user, password);
        }

        public Task<TUser?> FindByEmailAsync(string email)
        {
            return userManager.FindByEmailAsync(email);
        }

        public Task<TUser?> FindByIdAsync(string userId)
        {
            return userManager.FindByIdAsync(userId);
        }

        public Task<string> GetUserIdAsync(TUser user)
        {
            return userManager.GetUserIdAsync(user);
        }

        public Task<string> GetUserNameAsync(TUser user)
        {
            return userManager.GetUserNameAsync(user);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return userManager.IsInRoleAsync(user, roleName);
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            return userManager.GetRolesAsync(user);
        }
        public Task<IdentityResult> AddToRoleAsync(TUser user, string roleName)
        {
            return userManager.AddToRoleAsync(user, roleName);
        }
        public Task<IdentityResult> RemoveFromRoleAsync(TUser user, string roleName)
        {
            return userManager.RemoveFromRoleAsync(user, roleName);
        }
        public Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            return userManager.CreateAsync(user, password);
        }
        public Task<IdentityResult> CreateAsync(TUser user)
        {
            return userManager.CreateAsync(user);
        }

        public Task<IdentityResult> UpdateAsync(TUser user)
        {
            return userManager.UpdateAsync(user);
        }

        public Task<IdentityResult> DeleteAsync(TUser user)
        {
            return userManager.DeleteAsync(user);
        }
    }
}
