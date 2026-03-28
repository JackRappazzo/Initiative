using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenChallengeRatingFilter : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QueryFiltersOnCrQuarter)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnOnlyCrQuarterCreatures);

        [Given]
        public void QueryFiltersOnCrQuarter()
        {
            Query = new BestiarySearchQuery { BestiaryIds = [BestiaryId], ChallengeRating = "1/4" };
        }

        [Then]
        public void ShouldReturnOnlyCrQuarterCreatures()
        {
            Assert.That(Result.Count(), Is.EqualTo(2));
            Assert.That(Result.All(c => c.ChallengeRating == "1/4"), Is.True);
        }
    }
}
