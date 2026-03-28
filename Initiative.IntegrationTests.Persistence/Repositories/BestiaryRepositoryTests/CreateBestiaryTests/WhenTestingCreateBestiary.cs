using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.CreateBestiaryTests
{
    public abstract class WhenTestingCreateBestiary : WhenTestingBestiaryRepository
    {
        protected BestiaryDocument Bestiary;
        protected string Result;

        [When]
        public async Task CreateBestiaryIsCalled()
        {
            Result = await BestiaryRepository.CreateBestiary(Bestiary, CancellationToken);
        }
    }
}
