using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenSortByType : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QuerySortsByType)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnCreaturesOrderedByTypeThenName);

        [Given]
        public void QuerySortsByType()
        {
            Query = new BestiarySearchQuery { BestiaryIds = [BestiaryId], SortBy = CreatureSortBy.Type };
        }

        [Then]
        public void ShouldReturnCreaturesOrderedByTypeThenName()
        {
            // Seeded: Ancient Dragon (dragon), Goblin (humanoid), Goblin Boss (humanoid), Zombie (undead)
            // Expected order by type asc, then name asc:
            //   dragon:   Ancient Dragon
            //   humanoid: Goblin, Goblin Boss
            //   undead:   Zombie
            var names = Result.Select(c => c.Name).ToList();
            Assert.That(names, Is.EqualTo(new[] { "Ancient Dragon", "Goblin", "Goblin Boss", "Zombie" }));
        }
    }
}
