using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenSortByChallengeRating : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QuerySortsByChallengeRating)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnCreaturesOrderedByCrThenName);

        [Given]
        public void QuerySortsByChallengeRating()
        {
            Query = new BestiarySearchQuery { BestiaryIds = [BestiaryId], SortBy = CreatureSortBy.ChallengeRating };
        }

        [Then]
        public void ShouldReturnCreaturesOrderedByCrThenName()
        {
            // Seeded: Goblin (CR 1/4=0.25), Zombie (CR 1/4=0.25), Goblin Boss (CR 1), Ancient Dragon (CR 20)
            // Expected order by CR asc, then name asc for ties:
            //   CR 0.25: Goblin, Zombie  (G < Z alphabetically)
            //   CR 1:    Goblin Boss
            //   CR 20:   Ancient Dragon
            var names = Result.Select(c => c.Name).ToList();
            Assert.That(names, Is.EqualTo(new[] { "Goblin", "Zombie", "Goblin Boss", "Ancient Dragon" }));
        }
    }
}
