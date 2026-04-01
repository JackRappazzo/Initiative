using Initiative.Api.Core.Services.Parties;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Core.Services.PartyServiceTests
{
    public abstract class WhenTestingPartyService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected PartyService PartyService;

        [Dependency]
        protected IPartyRepository PartyRepository;

        protected CancellationToken CancellationToken = default;
    }
}
