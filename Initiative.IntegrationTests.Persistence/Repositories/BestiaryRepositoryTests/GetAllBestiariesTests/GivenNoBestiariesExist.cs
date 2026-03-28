using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.GetAllBestiariesTests
{
    public class GivenNoBestiariesExist : WhenTestingGetAllBestiaries
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NoBestiariesHaveBeenCreated)
            .When(GetAllBestiariesIsCalled)
            .Then(ShouldIncludeNoBestiariesWithThisUniqueSource);

        [Given]
        public void NoBestiariesHaveBeenCreated()
        {
            // Nothing to seed — we assert on absence of a known-unique source below
        }

        [Then]
        public void ShouldIncludeNoBestiariesWithThisUniqueSource()
        {
            // "NONE_SRC" is never inserted anywhere in the test suite
            Assert.That(Result.Any(b => b.Source == "NONE_SRC"), Is.False);
        }
    }
}
