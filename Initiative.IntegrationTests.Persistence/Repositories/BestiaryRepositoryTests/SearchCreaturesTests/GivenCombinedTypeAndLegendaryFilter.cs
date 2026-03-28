using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenCombinedTypeAndLegendaryFilter : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QueryCombinesTypeAndLegendary)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnMatchingCreature);

        [Given]
        public void QueryCombinesTypeAndLegendary()
        {
            Query = new BestiarySearchQuery
            {
                BestiaryIds = [BestiaryId],
                CreatureType = "humanoid",
                IsLegendary = false
            };
        }

        [Then]
        public void ShouldReturnMatchingCreature()
        {
            Assert.That(Result.Count(), Is.EqualTo(2));
            Assert.That(Result.All(c => c.CreatureType == "humanoid" && !c.IsLegendary), Is.True);
        }
    }
}
