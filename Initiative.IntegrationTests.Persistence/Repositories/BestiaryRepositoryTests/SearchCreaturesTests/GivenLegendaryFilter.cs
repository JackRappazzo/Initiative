using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenLegendaryFilter : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QueryFiltersOnLegendary)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnOnlyLegendaryCreatures);

        [Given]
        public void QueryFiltersOnLegendary()
        {
            Query = new BestiarySearchQuery { BestiaryIds = [BestiaryId], IsLegendary = true };
        }

        [Then]
        public void ShouldReturnOnlyLegendaryCreatures()
        {
            Assert.That(Result.Count(), Is.EqualTo(1));
            Assert.That(Result.Single().Name, Is.EqualTo("Ancient Dragon"));
        }
    }
}
