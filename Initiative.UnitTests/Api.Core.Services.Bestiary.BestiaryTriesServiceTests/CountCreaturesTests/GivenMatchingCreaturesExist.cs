using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.CountCreaturesTests
{
    public class GivenMatchingCreaturesExist : WhenTestingCountCreatures
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(QueryIsSet)
            .And(RepositoryReturnsCount)
            .When(CountCreaturesIsCalled)
            .Then(ShouldReturnCount)
            .And(ShouldForwardQueryToRepository);

        [Given]
        public void QueryIsSet()
        {
            Query = new BestiarySearchQuery
            {
                CreatureType = "humanoid",
                PageSize = 20,
                Skip = 0
            };
        }

        [Given]
        public void RepositoryReturnsCount()
        {
            BestiaryRepository.CountCreatures(Query, CancellationToken)
                .Returns(42L);
        }

        [Then]
        public void ShouldReturnCount()
        {
            Assert.That(Result, Is.EqualTo(42L));
        }

        [Then]
        public void ShouldForwardQueryToRepository()
        {
            BestiaryRepository.Received(1).CountCreatures(Query, CancellationToken);
        }
    }
}
