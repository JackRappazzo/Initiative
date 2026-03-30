using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.SearchCreaturesTests
{
    public class GivenNoMatchingCreaturesExist : WhenTestingSearchCreatures
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(QueryIsSet)
            .And(RepositoryReturnsEmpty)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnEmptyCollection);

        [Given]
        public void QueryIsSet()
        {
            Query = new BestiarySearchQuery
            {
                NameSearch = "Tarrasque",
                ChallengeRating = "30"
            };
        }

        [Given]
        public void RepositoryReturnsEmpty()
        {
            BestiaryRepository.SearchCreatures(Query, CancellationToken)
                .Returns(Enumerable.Empty<BestiaryCreatureDocument>());
        }

        [Then]
        public void ShouldReturnEmptyCollection()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Empty);
        }
    }
}
