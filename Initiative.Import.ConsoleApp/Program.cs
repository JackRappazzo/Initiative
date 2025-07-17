// See https://aka.ms/new-console-template for more information

using Initiative.Import.Core.Services;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Repositories;
using System.Threading;

var importer = new CreatureImporter(new CreatureMapper());
var bestiaryRepository = new BestiaryRepository(new DatabaseConnectionFactory());

var phbCreatures = await importer.ImportFromFile("Sources/PlayersHandbook24.json");
var mmCreatures = await importer.ImportFromFile("Sources/MonsterManual25.json");

CancellationToken cancellationToken = default;

var bestiaryService = new BestiaryImportService(importer, bestiaryRepository);

await bestiaryService.ImportSystemBestiary("Sources/PlayersHandbook24.json", "Players Handbook 2024", cancellationToken);
await bestiaryService.ImportSystemBestiary("Sources/MonsterManual25.json", "Monster Manual 2025", cancellationToken);
