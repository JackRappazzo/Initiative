using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.GetBestiaryBySourceTests
{
    public abstract class WhenTestingGetBestiaryBySource : WhenTestingBestiaryRepository
    {
        protected string Source;
        protected BestiaryDocument? Result;

        [When]
        public async Task GetBestiaryBySourceIsCalled()
        {
            Result = await BestiaryRepository.GetBestiaryBySource(Source, CancellationToken);
        }
    }
}
