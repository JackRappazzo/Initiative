using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenNameTypeAheadFilter : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QueryHasNamePrefix)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnOnlyMatchingCreatures);

        [Given]
        public void QueryHasNamePrefix()
        {
            Query = new BestiarySearchQuery { BestiaryIds = [BestiaryId], NameSearch = "Gob" };
        }

        [Then]
        public void ShouldReturnOnlyMatchingCreatures()
        {
            Assert.That(Result.Count(), Is.EqualTo(2));
            Assert.That(Result.All(c => c.Name.StartsWith("Goblin")), Is.True);
        }
    }
}
