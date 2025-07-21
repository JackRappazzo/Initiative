// See https://aka.ms/new-console-template for more information

using Initiative.Import.Core.Services;
using Initiative.Import.Core.Services.Extractors;
using Initiative.Persistence.Configuration;
using Initiative.Persistence.Repositories;
using System.Threading;

// Create all the extractor dependencies
var armorClassExtractor = new ArmorClassExtractor();
var speedExtractor = new SpeedExtractor();
var conditionImmunitiesExtractor = new ConditionImmunitiesExtractor();
var damageResistancesExtractor = new DamageResistancesExtractor();
var actionsExtractor = new ActionsExtractor();
var initiativeCalculator = new InitiativeCalculator();
var systemNameGenerator = new SystemNameGenerator();

// Create the creature mapper with all dependencies
var creatureMapper = new CreatureMapper(
    armorClassExtractor,
    speedExtractor,
    conditionImmunitiesExtractor,
    damageResistancesExtractor,
    actionsExtractor,
    initiativeCalculator,
    systemNameGenerator);

var importer = new CreatureImporter(creatureMapper);
var bestiaryRepository = new BestiaryRepository(new DatabaseConnectionFactory());

var phbCreatures = await importer.ImportFromFile("Sources/PlayersHandbook24.json");
var mmCreatures = await importer.ImportFromFile("Sources/MonsterManual25.json");

CancellationToken cancellationToken = default;

var bestiaryService = new BestiaryImportService(importer, bestiaryRepository);

await bestiaryService.ImportSystemBestiary("Sources/PlayersHandbook24.json", "Players Handbook 2024", cancellationToken);
await bestiaryService.ImportSystemBestiary("Sources/MonsterManual25.json", "Monster Manual 2025", cancellationToken);
