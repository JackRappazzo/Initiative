using Initiative.Api.Core.Services.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.RawDataBuilderTests
{
    public abstract class WhenTestingCustomCreatureRawDataBuilder : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected CustomCreatureRawDataBuilder Builder;
    }
}
