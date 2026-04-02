namespace Import.Bestiaries.Core.Services
{
    public interface ISpellImportService
    {
        Task ImportAsync(string spellSource, string spellSourceName, string resourceFileName, CancellationToken cancellationToken);
    }
}
