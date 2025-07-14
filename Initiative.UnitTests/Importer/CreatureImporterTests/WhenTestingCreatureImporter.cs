using Initiative.Import.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.CreatureImporterTests
{
    public abstract class WhenTestingCreatureImporter : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected CreatureImporter CreatureImporter;

        [Dependency]
        protected ICreatureMapper CreatureMapper;

        protected CancellationToken CancellationToken = default;
    }
}