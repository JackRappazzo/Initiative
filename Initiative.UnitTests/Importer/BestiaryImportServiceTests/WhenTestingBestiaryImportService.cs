using Initiative.Import.Core.Services;
using Initiative.Persistence.Models.Encounters;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Importer.BestiaryImportServiceTests
{
    public abstract class WhenTestingBestiaryImportService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected BestiaryImportService BestiaryImportService;

        [Dependency]
        protected ICreatureImporter CreatureImporter;

        [Dependency]
        protected IBestiaryRepository BestiaryRepository;

        protected CancellationToken CancellationToken = default;
    }
}