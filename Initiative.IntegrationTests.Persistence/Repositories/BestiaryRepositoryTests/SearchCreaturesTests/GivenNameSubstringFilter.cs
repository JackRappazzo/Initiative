using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.SearchCreaturesTests
{
    public class GivenNameSubstringFilter : GivenSeededBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreaturesExist)
            .And(QueryHasNameSubstring)
            .When(SearchCreaturesIsCalled)
            .Then(ShouldReturnOnlyMatchingCreatures);

        [Given]
        public void QueryHasNameSubstring()
        {
            // "lin" matches mid-word in "Goblin" and "Goblin Boss", but not "Ancient Dragon" or "Zombie"
            Query = new BestiarySearchQuery { BestiaryIds = [BestiaryId], NameSearch = "lin" };
        }

        [Then]
        public void ShouldReturnOnlyMatchingCreatures()
        {
            Assert.That(Result.Count(), Is.EqualTo(2));
            Assert.That(Result.All(c => c.Name.Contains("lin", StringComparison.OrdinalIgnoreCase)), Is.True);
        }
    }
}
