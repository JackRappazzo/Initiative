
namespace Initiative.Api.Services
{
    public interface IUserLoginService
    {
        Task<(bool success, string message, string? token)> LoginAndFetchToken(string email, string password, CancellationToken cancellationToken);
    }
}