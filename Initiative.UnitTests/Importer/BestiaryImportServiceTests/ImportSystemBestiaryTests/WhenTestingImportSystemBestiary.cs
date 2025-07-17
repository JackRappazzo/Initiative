using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Importer.BestiaryImportServiceTests.ImportSystemBestiaryTests
{
    public abstract class WhenTestingImportSystemBestiary : WhenTestingBestiaryImportService
    {
        protected string FilePath;
        protected string BestiaryName;

        [When(DoNotRethrowExceptions: true)]
        public async Task ImportSystemBestiaryIsCalled()
        {
            await BestiaryImportService.ImportSystemBestiary(FilePath, BestiaryName, CancellationToken);
        }
    }
}