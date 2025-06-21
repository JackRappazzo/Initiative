
namespace Initiative.Api.Core.Services.Users
{
    public interface IUserLoginService
    {
        Task<LoginResult> LoginAndFetchTokens(string email, string password, CancellationToken cancellationToken);
    }
}