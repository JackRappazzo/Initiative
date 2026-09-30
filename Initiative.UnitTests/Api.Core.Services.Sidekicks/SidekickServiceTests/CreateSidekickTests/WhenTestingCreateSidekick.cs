using Initiative.Api.Core.Services.Sidekicks;
using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickServiceTests.CreateSidekickTests
{
    public abstract class WhenTestingCreateSidekick : WhenTestingSidekickService
    {
        protected string PartyId;
        protected string OwnerId;
        protected string Name;
        protected SidekickData Data;
        protected PartyMember Result;

        [When(DoNotRethrowExceptions: true)]
        public async Task CreateSidekickIsCalled()
        {
            Result = await SidekickService.CreateSidekick(PartyId, OwnerId, Name, Data, CancellationToken);
        }
    }
}
