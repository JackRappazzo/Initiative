using Import.Bestiaries.Core.Services;

namespace Import.Bestiaries.ConsoleApp
{
    public class SourceProvider : ISourceProvider
    {
        private const string ResourceNamespace = "Import.Bestiaries.ConsoleApp.Sources";

        public Stream OpenSource(string resourceFileName)
        {
            var normalised = resourceFileName.Replace('/', '.').Replace('\\', '.');
            var resourceName = $"{ResourceNamespace}.{normalised}";
            var stream = typeof(SourceProvider).Assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
                throw new InvalidOperationException(
                    $"Embedded resource '{resourceName}' not found. " +
                    $"Ensure Sources/{resourceFileName} is marked as EmbeddedResource in the project file.");
            return stream;
        }
    }
}
