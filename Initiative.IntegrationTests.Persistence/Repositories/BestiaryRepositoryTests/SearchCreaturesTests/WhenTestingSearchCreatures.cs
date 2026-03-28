using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public abstract class WhenTestingSearchCreatures : WhenTestingBestiaryRepository
    {
        protected string BestiaryId;
        protected BestiarySearchQuery Query;
        protected IEnumerable<BestiaryCreatureDocument> Result;

        [When]
        public async Task SearchCreaturesIsCalled()
        {
            Result = await BestiaryRepository.SearchCreatures(Query, CancellationToken);
        }
    }
}
