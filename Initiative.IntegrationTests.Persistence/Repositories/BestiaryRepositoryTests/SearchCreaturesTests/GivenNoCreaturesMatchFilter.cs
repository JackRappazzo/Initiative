using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenNoCreaturesMatchFilter : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QueryUsesNonMatchingName)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnEmptyList);

        [Given]
        public void QueryUsesNonMatchingName()
        {
            Query = new BestiarySearchQuery { BestiaryIds = [BestiaryId], NameSearch = "Beholder" };
        }

        [Then]
        public void ShouldReturnEmptyList()
        {
            Assert.That(Result, Is.Empty);
        }
    }
}
