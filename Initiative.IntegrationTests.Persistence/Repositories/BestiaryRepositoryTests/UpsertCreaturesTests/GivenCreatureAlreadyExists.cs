using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.UpsertCreaturesTests
{
    public class GivenCreatureAlreadyExists : WhenTestingUpsertCreatures
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryExists)
            .And(CreatureAlreadyUpserted)
            .And(CreaturesAreSetWithUpdatedData)
            .When(UpsertCreaturesIsCalled)
            .Then(ShouldNotCreateDuplicate)
            .And(ShouldUpdateExistingCreature);

        [Given]
        public async Task BestiaryExists()
        {
            BestiaryId = await BestiaryRepository.CreateBestiary(BuildBestiary(source: "UP2"), CancellationToken);
        }

        [Given]
        public async Task CreatureAlreadyUpserted()
        {
            await BestiaryRepository.UpsertCreatures(
                [BuildCreature(BestiaryId, name: "Orc", creatureType: "humanoid", cr: "1/2")],
                CancellationToken);
        }

        [Given]
        public void CreaturesAreSetWithUpdatedData()
        {
            // Same name — should replace, not insert a second document
            Creatures = [BuildCreature(BestiaryId, name: "Orc", creatureType: "humanoid", cr: "1")];
        }

        [Then]
        public async Task ShouldNotCreateDuplicate()
        {
            var results = await BestiaryRepository.SearchCreatures(
                new BestiarySearchQuery { BestiaryIds = [BestiaryId], NameSearch = "Orc" },
                CancellationToken);

            Assert.That(results.Count(), Is.EqualTo(1));
        }

        [Then]
        public async Task ShouldUpdateExistingCreature()
        {
            var results = await BestiaryRepository.SearchCreatures(
                new BestiarySearchQuery { BestiaryIds = [BestiaryId], NameSearch = "Orc" },
                CancellationToken);

            Assert.That(results.First().ChallengeRating, Is.EqualTo("1"));
        }
    }
}
