using Initiative.Persistence.Models.Bestiary;
using Initiative.Persistence.Repositories;
using MongoDB.Bson;

namespace Initiative.Api.Core.Services.Bestiary
{
    public class BestiaryService : IBestiaryService
    {
        private readonly IBestiaryRepository _repository;

        public BestiaryService(IBestiaryRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<BestiaryDocument>> GetAvailableBestiaries(string userId, CancellationToken cancellationToken)
            => _repository.GetBestariesByOwners(new string?[] { null, userId }, cancellationToken);

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

        public async Task<BestiaryDocument> CreateCustomBestiary(string userId, string name, CancellationToken cancellationToken)
        {
            var doc = new BestiaryDocument { Name = name, OwnerId = userId };
            await _repository.CreateBestiary(doc, cancellationToken);
            return doc;
        }

        public async Task RenameBestiary(string bestiaryId, string userId, string name, CancellationToken cancellationToken)
        {
            var bestiary = await _repository.GetBestiaryById(bestiaryId, cancellationToken);
            if (bestiary == null || bestiary.OwnerId != userId)
                throw new ArgumentException("Bestiary not found.", nameof(bestiaryId));
            await _repository.RenameBestiary(bestiaryId, userId, name, cancellationToken);
        }

        public async Task DeleteBestiary(string bestiaryId, string userId, CancellationToken cancellationToken)
        {
            var bestiary = await _repository.GetBestiaryById(bestiaryId, cancellationToken);
            if (bestiary == null || bestiary.OwnerId != userId)
                throw new ArgumentException("Bestiary not found.", nameof(bestiaryId));
            await _repository.DeleteBestiary(bestiaryId, userId, cancellationToken);
        }

        public async Task<BestiaryCreatureDocument> CreateCustomCreature(string bestiaryId, string userId, CustomCreatureData data, CancellationToken cancellationToken)
        {
            var bestiary = await _repository.GetBestiaryById(bestiaryId, cancellationToken);
            if (bestiary == null || bestiary.OwnerId != userId)
                throw new ArgumentException("Bestiary not found.", nameof(bestiaryId));

            var doc = new BestiaryCreatureDocument
            {
                BestiaryId = bestiaryId,
                Name = data.Name,
                CreatureType = data.CreatureType,
                ChallengeRating = data.ChallengeRating,
                IsLegendary = data.IsLegendary,
                RawData = BuildRawData(data)
            };

            await _repository.InsertCreature(doc, cancellationToken);
            return doc;
        }

        public async Task UpdateCustomCreature(string creatureId, string bestiaryId, string userId, CustomCreatureData data, CancellationToken cancellationToken)
        {
            var bestiary = await _repository.GetBestiaryById(bestiaryId, cancellationToken);
            if (bestiary == null || bestiary.OwnerId != userId)
                throw new ArgumentException("Bestiary not found.", nameof(bestiaryId));

            var existing = await _repository.GetCreatureById(creatureId, cancellationToken)
                ?? throw new ArgumentException("Creature not found.", nameof(creatureId));

            existing.Name = data.Name;
            existing.CreatureType = data.CreatureType;
            existing.ChallengeRating = data.ChallengeRating;
            existing.IsLegendary = data.IsLegendary;
            existing.RawData = BuildRawData(data);

            await _repository.UpdateCreature(existing, cancellationToken);
        }

        public Task DeleteCustomCreature(string creatureId, CancellationToken cancellationToken)
            => _repository.DeleteCreature(creatureId, cancellationToken);

        private static BsonDocument BuildRawData(CustomCreatureData data)
        {
            var doc = new BsonDocument
            {
                { "name", data.Name },
                { "type", data.CreatureType != null ? (BsonValue)data.CreatureType : BsonNull.Value },
                { "cr",   data.ChallengeRating != null ? (BsonValue)data.ChallengeRating : BsonNull.Value },
            };

            if (data.HP.HasValue)
                doc["hp"] = new BsonDocument { { "average", data.HP.Value } };

            if (data.AC.HasValue)
                doc["ac"] = new BsonArray { data.AC.Value };

            if (!string.IsNullOrWhiteSpace(data.Traits))
                doc["trait"] = new BsonArray
                {
                    new BsonDocument
                    {
                        { "name", "Traits" },
                        { "entries", new BsonArray { data.Traits } }
                    }
                };

            return doc;
        }
    }
}
