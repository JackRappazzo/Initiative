
namespace Initiative.Api.Services
{
    public interface IUserRegistrationService
    {
        Task<(bool, string)> RegisterUser(string displayName, string email, string password, CancellationToken cancellationToken);
    }
}