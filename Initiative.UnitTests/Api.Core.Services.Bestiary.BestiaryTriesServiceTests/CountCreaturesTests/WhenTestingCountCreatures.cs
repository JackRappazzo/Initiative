using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.CountCreaturesTests
{
    public abstract class WhenTestingCountCreatures : WhenTestingBestiaryService
    {
        protected BestiarySearchQuery Query;
        protected long Result;

        [When]
        public async Task CountCreaturesIsCalled()
        {
            Result = await BestiaryService.CountCreatures(Query, CancellationToken);
        }
    }
}
