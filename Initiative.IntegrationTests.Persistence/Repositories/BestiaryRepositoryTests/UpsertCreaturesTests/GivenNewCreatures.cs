using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.UpsertCreaturesTests
{
    public class GivenNewCreatures : WhenTestingUpsertCreatures
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryExists)
            .And(CreaturesAreSet)
            .When(UpsertCreaturesIsCalled)
            .Then(ShouldPersistCreatures);

        [Given]
        public async Task BestiaryExists()
        {
            BestiaryId = await BestiaryRepository.CreateBestiary(BuildBestiary(source: "UP1"), CancellationToken);
        }

        [Given]
        public void CreaturesAreSet()
        {
            Creatures =
            [
                BuildCreature(BestiaryId, name: "Goblin", creatureType: "humanoid", cr: "1/4"),
                BuildCreature(BestiaryId, name: "Dragon", creatureType: "dragon", cr: "17", isLegendary: true)
            ];
        }

        [Then]
        public async Task ShouldPersistCreatures()
        {
            var results = await BestiaryRepository.SearchCreatures(
                new BestiarySearchQuery { BestiaryIds = [BestiaryId] },
                CancellationToken);

            Assert.That(results.Count(), Is.EqualTo(2));
        }
    }
}
