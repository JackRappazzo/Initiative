namespace Import.Bestiaries.Core.Services
{
    public interface IBestiaryImportService
    {
        Task ImportAsync(CancellationToken cancellationToken);
    }
}
