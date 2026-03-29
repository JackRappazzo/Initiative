using Import.Bestiaries.Core.Parsing;
using Import.Bestiaries.Core.Services;
using Import.Bestiaries.ConsoleApp;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddScoped<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
services.AddScoped<IBestiaryRepository, BestiaryRepository>();
services.AddScoped<IFivEToolsParser, FivEToolsParser>();
services.AddScoped<ISourceProvider, SourceProvider>();
services.AddScoped<IBestiaryImportService, BestiaryImportService>();

await using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var importer = scope.ServiceProvider.GetRequiredService<IBestiaryImportService>();

try
{
    await importer.ImportAsync(CancellationToken.None);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Import failed: {ex.Message}");
    Environment.Exit(1);
}

