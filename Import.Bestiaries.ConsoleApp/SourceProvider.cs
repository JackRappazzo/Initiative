using Import.Bestiaries.Core.Services;

namespace Import.Bestiaries.ConsoleApp
{
    public class SourceProvider : ISourceProvider
    {
        private const string ResourceName = "Import.Bestiaries.ConsoleApp.Sources.MonsterManual25.json";

        public Stream OpenMonsterManual25()
        {
            var stream = typeof(SourceProvider).Assembly.GetManifestResourceStream(ResourceName);
            if (stream is null)
                throw new InvalidOperationException(
                    $"Embedded resource '{ResourceName}' not found. " +
                    "Ensure Sources/MonsterManual25.json is marked as EmbeddedResource in the project file.");
            return stream;
        }
    }
}
