using Initiative.Persistence.Models.Parties;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Core.Services.PartyServiceTests.CreatePartyTests
{
    public abstract class WhenTestingCreateParty : WhenTestingPartyService
    {
        protected string OwnerId;
        protected string Name;
        protected IEnumerable<PartyMember> Members;
        protected string Result;

        [When]
        public async Task CreatePartyIsCalled()
        {
            Result = await PartyService.CreateParty(OwnerId, Name, Members, CancellationToken);
        }

        [Then]
        public void ResultShouldNotBeNullOrEmpty()
        {
            Assert.That(string.IsNullOrEmpty(Result), Is.False, "Result should not be null or empty.");
        }
    }
}
