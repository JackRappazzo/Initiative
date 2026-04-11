using Initiative.Api.Core.Services.Bestiary;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;
using MongoDB.Bson;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests
{
    public abstract class WhenTestingBestiaryService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected BestiaryService BestiaryService;

        [Dependency]
        protected IBestiaryRepository BestiaryRepository;

        [Dependency]
        protected ICustomCreatureRawDataBuilder CustomCreatureRawDataBuilder;

        protected CancellationToken CancellationToken = default;

        protected static string NewId() => ObjectId.GenerateNewId().ToString();
    }
}
