using Import.Bestiaries.Core.Parsing;
using Import.Bestiaries.Core.Services;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Import.BestiaryImportServiceTests
{
    public abstract class WhenTestingBestiaryImportService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected BestiaryImportService BestiaryImportService;

        [Dependency]
        protected IBestiaryRepository BestiaryRepository;

        [Dependency]
        protected IFivEToolsParser Parser;

        [Dependency]
        protected ISourceProvider SourceProvider;

        protected CancellationToken CancellationToken = default;

        protected static Stream JsonStream(string json)
            => new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

        protected static string MinimalMonsterJson() => """
            {
              "monster": [
                { "name": "Goblin", "type": "humanoid", "cr": "1/4" }
              ]
            }
            """;
    }
}

