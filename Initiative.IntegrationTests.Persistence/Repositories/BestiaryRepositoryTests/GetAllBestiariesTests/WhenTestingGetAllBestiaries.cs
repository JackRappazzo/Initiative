using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.GetAllBestiariesTests
{
    public abstract class WhenTestingGetAllBestiaries : WhenTestingBestiaryRepository
    {
        protected IEnumerable<BestiaryDocument> Result;

        [When]
        public async Task GetAllBestiariesIsCalled()
        {
            Result = await BestiaryRepository.GetBestariesByOwners(new string?[] { null }, CancellationToken);
        }
    }
}
