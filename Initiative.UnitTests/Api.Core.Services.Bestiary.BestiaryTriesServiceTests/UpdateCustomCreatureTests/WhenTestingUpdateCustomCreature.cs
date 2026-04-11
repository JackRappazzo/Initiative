using Initiative.Api.Core.Services.Bestiary;
using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.UpdateCustomCreatureTests
{
    public abstract class WhenTestingUpdateCustomCreature : WhenTestingBestiaryService
    {
        protected string CreatureId;
        protected string BestiaryId;
        protected string UserId;
        protected CustomCreatureData Data;
        protected BsonDocument RawData;
        protected BestiaryCreatureDocument ExistingCreature;

        [When]
        public async Task UpdateCustomCreatureIsCalled()
        {
            await BestiaryService.UpdateCustomCreature(CreatureId, BestiaryId, UserId, Data, CancellationToken);
        }
    }
}
