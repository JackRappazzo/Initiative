// See https://aka.ms/new-console-template for more information

using Initiative.Import.Core.Services;

var importer = new CreatureImporter(new CreatureMapper());

var creatures = await importer.ImportFromFile("Sources/PlayersHandbook24.json");

foreach (var creature in creatures)
{
    Console.WriteLine($"Name: {creature.Name}, AC: {creature.ArmorClass}, HP: {creature.HitPoints}");
}