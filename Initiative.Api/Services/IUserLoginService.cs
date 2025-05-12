

namespace Initiative.Api.Services
{
    public interface IUserLoginService
    {
        Task<LoginResult> LoginAndFetchTokens(string email, string password, CancellationToken cancellationToken);
    }
}