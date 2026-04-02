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

var sources = new[]
{
    (Source: "XMM",     Name: "Monster Manual 2025",                File: "MonsterManual25.json"),
    (Source: "XDMG",    Name: "Dungeon Masters Guide (2024)",       File: "DMG2024.json"),
    (Source: "XPHB",    Name: "Player's Handbook (2024)",           File: "PHB2024.json"),
    (Source: "FM!",     Name: "Flee, Mortals!",                     File: "FleeMortals.json"),
    (Source: "MPMM",    Name: "Monsters of the Multiverse (2024)",  File: "MonstersOfTheMultiverse2024.json"),
};

foreach (var (source, name, file) in sources)
{
    try
    {
        await importer.ImportAsync(source, name, file, CancellationToken.None);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Import of '{name}' failed: {ex.Message}");
        Environment.Exit(1);
    }
}

