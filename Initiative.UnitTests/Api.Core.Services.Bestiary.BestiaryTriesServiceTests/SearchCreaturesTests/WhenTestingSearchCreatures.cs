using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.SearchCreaturesTests
{
    public abstract class WhenTestingSearchCreatures : WhenTestingBestiaryService
    {
        protected BestiarySearchQuery Query;
        protected IEnumerable<BestiaryCreatureDocument> Result;

        [When]
        public async Task SearchCreaturesIsCalled()
        {
            Result = await BestiaryService.SearchCreatures(Query, CancellationToken);
        }
    }
}
