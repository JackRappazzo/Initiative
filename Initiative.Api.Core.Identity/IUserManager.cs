using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Initiative.Api.Core.Identity
{
    public interface IUserManager<TUser> where TUser : ApplicationIdentity
    {
        Task<bool> CheckPasswordAsync(TUser user, string password);
        Task<TUser?> FindByEmailAsync(string email);
        Task<TUser?> FindByIdAsync(string userId);
        Task<string> GetUserIdAsync(TUser user);
        Task<string> GetUserNameAsync(TUser user);
        Task<bool> IsInRoleAsync(TUser user, string roleName);
        Task<IList<string>> GetRolesAsync(TUser user);
        Task<IdentityResult> AddToRoleAsync(TUser user, string roleName);
        Task<IdentityResult> RemoveFromRoleAsync(TUser user, string roleName);
        Task<IdentityResult> CreateAsync(TUser user, string password);
        Task<IdentityResult> CreateAsync(TUser user);
        Task<IdentityResult> UpdateAsync(TUser user);
        Task<IdentityResult> DeleteAsync(TUser user);
    }
}
