using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.GetBestiaryBySourceTests
{
    public class GivenBestiaryExists : WhenTestingGetBestiaryBySource
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryIsCreated)
            .And(SourceIsSet)
            .When(GetBestiaryBySourceIsCalled)
            .Then(ShouldReturnBestiary);

        [Given]
        public async Task BestiaryIsCreated()
        {
            await BestiaryRepository.CreateBestiary(BuildBestiary(name: "Player's Handbook", source: "PHB"), CancellationToken);
        }

        [Given]
        public void SourceIsSet()
        {
            Source = "PHB";
        }

        [Then]
        public void ShouldReturnBestiary()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result!.Source, Is.EqualTo("PHB"));
            Assert.That(Result.Name, Is.EqualTo("Player's Handbook"));
            Assert.That(Result.IsSystem, Is.True);
        }
    }

}
