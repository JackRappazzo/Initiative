
namespace Initiative.Api.Core.Services.Users
{
    public interface IUserService
    {
        Task<(bool, string)> RegisterUser(string displayName, string email, string password, CancellationToken cancellationToken);
    }
}