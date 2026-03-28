using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.UpsertCreaturesTests
{
    public abstract class WhenTestingUpsertCreatures : WhenTestingBestiaryRepository
    {
        protected string BestiaryId;
        protected List<BestiaryCreatureDocument> Creatures;

        [When]
        public async Task UpsertCreaturesIsCalled()
        {
            await BestiaryRepository.UpsertCreatures(Creatures, CancellationToken);
        }
    }
}
