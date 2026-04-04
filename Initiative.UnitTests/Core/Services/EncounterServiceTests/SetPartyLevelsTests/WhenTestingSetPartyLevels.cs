using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.SetPartyLevelsTests
{
    public abstract class WhenTestingSetPartyLevels : WhenTestingEncounterService
    {
        protected string EncounterId;
        protected string OwnerId;
        protected IEnumerable<int> PartyLevels;

        [When(DoNotRethrowExceptions:true)]
        public async Task SetPartyLevelsIsCalled()
        {
            await EncounterService.SetEncounterPartyLevels(EncounterId, OwnerId, PartyLevels, CancellationToken);
        }
    }
}
