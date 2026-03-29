using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Import.BestiaryImportServiceTests.ImportAsyncTests
{
    public abstract class WhenTestingImportAsync : WhenTestingBestiaryImportService
    {
        [When]
        public async Task ImportAsyncIsCalled()
        {
            await BestiaryImportService.ImportAsync(CancellationToken);
        }
    }
}
