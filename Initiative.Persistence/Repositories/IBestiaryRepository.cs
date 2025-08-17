using Initiative.Persistence.Models.Encounters;
using Initiative.Persistence.Models.Encounters.Dtos;

namespace Initiative.Persistence.Repositories
{
    public interface IBestiaryRepository
    {
        Task<string> AddCreature(string beastiaryId, Creature creature, CancellationToken cancellationToken);
        Task<string> CreateSystemBestiary(string name, CancellationToken cancellationToken);
        Task<string> CreateUserBestiary(string? ownerId, string name, CancellationToken cancellationToken);
        Task<Bestiary> GetSystemBestiary(string bestiaryId, CancellationToken cancellationToken);
        Task<Bestiary> GetSystemBestiaryByName(string name, CancellationToken cancellationToken);
        Task<Bestiary> GetUserBestiary(string bestiaryId, string ownerId, CancellationToken cancellationToken);
        Task<string> UpsertSystemBestiary(string name, IEnumerable<Creature> creatures, CancellationToken cancellationToken);
        Task<IEnumerable<GetAvailableBestiaryDto>> GetAvailableBestiaries(string userId, CancellationToken cancellationToken);
    }
}