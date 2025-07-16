using Initiative.Persistence.Models.Encounters;

namespace Initiative.Persistence.Repositories
{
    public interface IBestiaryRepository
    {
        Task<string> AddCreature(string beastiaryId, Creature creature, CancellationToken cancellationToken);
        Task<string> CreateSystemBestiary(string name, CancellationToken cancellationToken);
        Task<string> CreateUserBestiary(string? ownerId, string name, CancellationToken cancellationToken);
        Task<Beastiary> GetSystemBestiary(string beastiaryId, CancellationToken cancellationToken);
        Task<Beastiary> GetUserBestiary(string beastiaryId, string ownerId, CancellationToken cancellationToken);
    }
}