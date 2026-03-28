using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    /// <summary>
    /// Base for search tests that need a pre-populated bestiary.
    /// Seeds: Goblin (humanoid, CR 1/4), Goblin Boss (humanoid, CR 1),
    ///        Ancient Dragon (dragon, CR 20, legendary), Zombie (undead, CR 1/4).
    /// Each concrete subclass gets its own unique source code to avoid unique-index collisions
    /// when multiple fixtures run in the same test session.
    /// </summary>
    public abstract class GivenSeededBestiary : WhenTestingSearchCreatures
    {
        // Unique per concrete type so the Source unique-index is never violated
        private string UniqueSource => GetType().Name[..Math.Min(GetType().Name.Length, 8)].ToUpper();

        [Given]
        public async Task BestiaryAndCreaturesExist()
        {
            BestiaryId = await BestiaryRepository.CreateBestiary(BuildBestiary(source: UniqueSource), CancellationToken);

            await BestiaryRepository.UpsertCreatures(
            [
                BuildCreature(BestiaryId, name: "Goblin",         creatureType: "humanoid", cr: "1/4",  isLegendary: false),
                BuildCreature(BestiaryId, name: "Goblin Boss",    creatureType: "humanoid", cr: "1",    isLegendary: false),
                BuildCreature(BestiaryId, name: "Ancient Dragon", creatureType: "dragon",   cr: "20",   isLegendary: true),
                BuildCreature(BestiaryId, name: "Zombie",         creatureType: "undead",   cr: "1/4",  isLegendary: false)
            ], CancellationToken);
        }
    }
}
