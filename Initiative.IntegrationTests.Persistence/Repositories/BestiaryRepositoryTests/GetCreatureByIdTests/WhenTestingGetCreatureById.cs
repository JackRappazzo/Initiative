using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.GetCreatureByIdTests
{
    public abstract class WhenTestingGetCreatureById : WhenTestingBestiaryRepository
    {
        protected string CreatureId;
        protected BestiaryCreatureDocument? Result;

        [When]
        public async Task GetCreatureByIdIsCalled()
        {
            Result = await BestiaryRepository.GetCreatureById(CreatureId, CancellationToken);
        }
    }
}
