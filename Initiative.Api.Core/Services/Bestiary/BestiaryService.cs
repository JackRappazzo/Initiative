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

        public async Task<SearchCreaturesResult> SearchCreatures(BestiarySearchQuery query, CancellationToken cancellationToken)
        {
            var countQuery = new BestiarySearchQuery
            {
                BestiaryIds = query.BestiaryIds,
                NameSearch = query.NameSearch,
                CreatureType = query.CreatureType,
                ChallengeRating = query.ChallengeRating,
                IsLegendary = query.IsLegendary,
                OwnerId = query.OwnerId,
            };

            var creaturesTask = _repository.SearchCreatures(query, cancellationToken);
            var countTask = _repository.CountCreatures(countQuery, cancellationToken);
            await Task.WhenAll(creaturesTask, countTask);

            return new SearchCreaturesResult { Creatures = creaturesTask.Result, TotalCount = countTask.Result };
        }

        public Task<BestiaryCreatureDocument?> GetCreatureById(string creatureId, CancellationToken cancellationToken)
            => _repository.GetCreatureById(creatureId, cancellationToken);
    }
}
