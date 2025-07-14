using Initiative.Import.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.CreatureMapperTests
{
    public abstract class WhenTestingCreatureMapper : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected CreatureMapper CreatureMapper;
    }
}