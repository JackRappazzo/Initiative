using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Initiative.UnitTests.Api.Helpers
{
    public class MockUserManager<T> : UserManager<T> where T:class
    {
        public MockUserManager(IUserStore<T> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<T> passwordHasher, IEnumerable<IUserValidator<T>> userValidators, IEnumerable<IPasswordValidator<T>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<T>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public MockUserManager() : base(new MockUserStore<T>(), null, null, null, null, null, null, null, null)
        {

        }
    }

    public class MockUserStore<T> : IUserStore<T> where T:class
    {
        public void Dispose() { }

        public Task<string> GetUserIdAsync(T user, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken) => Task.FromResult<T>(null);
        public Task<T> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => Task.FromResult<T>(null);
    }
}
