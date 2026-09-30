using Initiative.Api.Core.Services.Sidekicks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Core.Services.Sidekicks.SidekickStatBlockBuilderTests
{
    public abstract class WhenTestingSidekickStatBlockBuilder : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected SidekickStatBlockBuilder Builder;
    }
}
