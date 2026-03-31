using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.GetCreatureByIdTests
{
    public abstract class WhenTestingGetCreatureById : WhenTestingBestiaryService
    {
        protected string CreatureId;
        protected BestiaryCreatureDocument? Result;

        [When]
        public async Task GetCreatureByIdIsCalled()
        {
            Result = await BestiaryService.GetCreatureById(CreatureId, CancellationToken);
        }
    }
}
