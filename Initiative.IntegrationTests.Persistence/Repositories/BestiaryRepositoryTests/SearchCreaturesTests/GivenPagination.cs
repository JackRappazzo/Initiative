using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenPagination : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QueryHasPagination)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldRespectPageSize);

        [Given]
        public void QueryHasPagination()
        {
            Query = new BestiarySearchQuery { BestiaryIds = [BestiaryId], PageSize = 2, Skip = 0 };
        }

        [Then]
        public void ShouldRespectPageSize()
        {
            Assert.That(Result.Count(), Is.EqualTo(2));
        }
    }
}
