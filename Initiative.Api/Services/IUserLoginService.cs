
namespace Initiative.Api.Services
{
    public interface IUserLoginService
    {
        Task<(bool success, string message)> Login(string email, string password, CancellationToken cancellationToken);
    }
}