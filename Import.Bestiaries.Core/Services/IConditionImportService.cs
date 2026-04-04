namespace Import.Bestiaries.Core.Services
{
    public interface IConditionImportService
    {
        /// <summary>
        /// Imports conditions from a 5etools conditionsdiseases.json file.
        /// </summary>
        Task ImportAsync(string source, string resourceFileName, CancellationToken cancellationToken);
    }
}
