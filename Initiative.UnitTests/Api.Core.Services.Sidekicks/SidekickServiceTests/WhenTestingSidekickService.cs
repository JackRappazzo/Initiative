using Initiative.Api.Core.Services.Sidekicks;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickServiceTests
{
    public abstract class WhenTestingSidekickService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected SidekickService SidekickService;

        [Dependency]
        protected IPartyRepository PartyRepository;

        [Dependency]
        protected IBestiaryRepository BestiaryRepository;

        [Dependency]
        protected ISidekickStatBlockBuilder StatBlockBuilder;

        protected CancellationToken CancellationToken = default;
    }
}
