using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenCreatureTypeFilter : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QueryFiltersOnDragon)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnOnlyDragons);

        [Given]
        public void QueryFiltersOnDragon()
        {
            Query = new BestiarySearchQuery { BestiaryIds = [BestiaryId], CreatureType = "dragon" };
        }

        [Then]
        public void ShouldReturnOnlyDragons()
        {
            Assert.That(Result.Count(), Is.EqualTo(1));
            Assert.That(Result.Single().Name, Is.EqualTo("Ancient Dragon"));
        }
    }
}
