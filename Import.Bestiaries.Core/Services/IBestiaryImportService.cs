namespace Import.Bestiaries.Core.Services
{
    public interface IBestiaryImportService
    {
        Task ImportAsync(string bestiarySource, string bestiaryName, string resourceFileName, CancellationToken cancellationToken);
    }
}
