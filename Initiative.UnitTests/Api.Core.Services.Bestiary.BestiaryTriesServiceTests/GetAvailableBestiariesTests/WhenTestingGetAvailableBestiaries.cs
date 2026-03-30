using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.GetAvailableBestiariesTests
{
    public abstract class WhenTestingGetAvailableBestiaries : WhenTestingBestiaryService
    {
        protected string UserId;
        protected IEnumerable<BestiaryDocument> Result;

        [When]
        public async Task GetAvailableBestiariesIsCalled()
        {
            Result = await BestiaryService.GetAvailableBestiaries(UserId, CancellationToken);
        }
    }
}
