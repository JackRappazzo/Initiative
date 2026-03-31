using Initiative.Persistence.Models.Bestiary;
using Initiative.Persistence.Repositories;

namespace Initiative.Api.Core.Services.Bestiary
{
    public class BestiaryService : IBestiaryService
    {
        private readonly IBestiaryRepository _repository;

        public BestiaryService(IBestiaryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BestiaryDocument>> GetAvailableBestiaries(string userId, CancellationToken cancellationToken)
        {
            var system = await _repository.GetAllBestiaries(cancellationToken);
            var custom = await _repository.GetBestariesByOwner(userId, cancellationToken);
            return system.Concat(custom).OrderBy(b => b.Name);
        }

        public Task<IEnumerable<BestiaryCreatureDocument>> SearchCreatures(BestiarySearchQuery query, CancellationToken cancellationToken)
            => _repository.SearchCreatures(query, cancellationToken);

        public Task<long> CountCreatures(BestiarySearchQuery query, CancellationToken cancellationToken)
            => _repository.CountCreatures(query, cancellationToken);
    }
}
