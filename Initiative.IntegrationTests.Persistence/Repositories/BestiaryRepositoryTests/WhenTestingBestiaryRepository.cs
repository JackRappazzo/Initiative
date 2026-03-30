using Initiative.IntegrationTests.Persistence.Utilities;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Models.Bestiary;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests
{
    public abstract class WhenTestingBestiaryRepository : WhenTestingWithMongoDb
    {
        [ItemUnderTest] protected BestiaryRepository BestiaryRepository;
        [Dependency] public IDatabaseConnectionFactory DatabaseConnectionFactory = new TestConnectionFactory();

        protected CancellationToken CancellationToken = default;

        // ── Shared helpers ────────────────────────────────────────────────────

        protected BestiaryDocument BuildBestiary(string name = "Test Bestiary", string source = "TST")
            => new BestiaryDocument
            {
                Name = name,
                Source = source,
                OwnerId = null
            };

        protected BestiaryDocument BuildCustomBestiary(string ownerId, string name = "My Custom Bestiary")
            => new BestiaryDocument
            {
                Name = name,
                Source = null,
                OwnerId = ownerId
            };

        protected BestiaryCreatureDocument BuildCreature(string bestiaryId, string name = "Test Creature",
            string creatureType = "humanoid", string cr = "1", bool isLegendary = false)
            => new BestiaryCreatureDocument
            {
                BestiaryId = bestiaryId,
                Source = "TST",
                Name = name,
                CreatureType = creatureType,
                ChallengeRating = cr,
                IsLegendary = isLegendary,
                RawData = new BsonDocument { { "name", name }, { "type", creatureType }, { "cr", cr } }
            };
    }
}
