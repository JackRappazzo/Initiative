using Initiative.Api.Core.Services.Bestiary;
using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.CreateCustomCreatureTests
{
    public abstract class WhenTestingCreateCustomCreature : WhenTestingBestiaryService
    {
        protected string BestiaryId;
        protected string UserId;
        protected CustomCreatureData Data;
        protected BestiaryCreatureDocument Result;
        protected BsonDocument RawData;

        [When]
        public async Task CreateCustomCreatureIsCalled()
        {
            Result = await BestiaryService.CreateCustomCreature(BestiaryId, UserId, Data, CancellationToken);
        }
    }
}
