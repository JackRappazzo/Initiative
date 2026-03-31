using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.CountCreaturesTests
{
    public class GivenNoMatchingCreaturesExist : WhenTestingCountCreatures
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(QueryIsSet)
            .And(RepositoryReturnsZero)
            .When(CountCreaturesIsCalled)
            .Then(ShouldReturnZero);

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
        public void RepositoryReturnsZero()
        {
            BestiaryRepository.CountCreatures(Query, CancellationToken)
                .Returns(0L);
        }

        [Then]
        public void ShouldReturnZero()
        {
            Assert.That(Result, Is.EqualTo(0L));
        }
    }
}
