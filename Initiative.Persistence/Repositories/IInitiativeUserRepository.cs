using Initiative.Persistence.Models;

namespace Initiative.Persistence.Repositories
{
    public interface IInitiativeUserRepository
    {
        Task<InitiativeUserModel> FetchUserByIdentityId(string identityId, CancellationToken cancellationToken);
        Task<string> InsertUser(InitiativeUserModel user, CancellationToken cancellationToken);
        Task<bool> RoomCodeExists(string roomCode, CancellationToken cancellationToken);
        Task<bool> UserExists(string identityId, CancellationToken cancellationToken);
    }
}