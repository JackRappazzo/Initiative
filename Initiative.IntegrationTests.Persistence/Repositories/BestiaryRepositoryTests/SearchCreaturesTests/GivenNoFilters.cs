using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenNoFilters : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QueryHasNoFilters)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnAllCreatures)
            .And(ShouldReturnCreaturesOrderedByName);

        [Given]
        public void QueryHasNoFilters()
        {
            Query = new BestiarySearchQuery { BestiaryIds = [BestiaryId] };
        }

        [Then]
        public void ShouldReturnAllCreatures()
        {
            Assert.That(Result.Count(), Is.EqualTo(4));
        }

        [Then]
        public void ShouldReturnCreaturesOrderedByName()
        {
            var names = Result.Select(c => c.Name).ToList();
            Assert.That(names, Is.EqualTo(names.OrderBy(n => n).ToList()));
        }
    }
}
