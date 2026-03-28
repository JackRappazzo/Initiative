using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.GetAllBestiariesTests
{
    public class GivenMultipleBestiariesExist : WhenTestingGetAllBestiaries
    {
        private string _sourceA = "GAL1";
        private string _sourceB = "GAL2";

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiariesAreCreated)
            .When(GetAllBestiariesIsCalled)
            .Then(ShouldContainCreatedBestiaries)
            .And(ShouldReturnBestiariesOrderedByName);

        [Given]
        public async Task BestiariesAreCreated()
        {
            await BestiaryRepository.CreateBestiary(BuildBestiary(name: "Xanathar's Guide", source: _sourceA), CancellationToken);
            await BestiaryRepository.CreateBestiary(BuildBestiary(name: "Monster Manual", source: _sourceB), CancellationToken);
        }

        [Then]
        public void ShouldContainCreatedBestiaries()
        {
            var sources = Result.Select(b => b.Source).ToList();
            Assert.That(sources, Does.Contain(_sourceA));
            Assert.That(sources, Does.Contain(_sourceB));
        }

        [Then]
        public void ShouldReturnBestiariesOrderedByName()
        {
            var names = Result.Select(b => b.Name).ToList();
            Assert.That(names, Is.EqualTo(names.OrderBy(n => n).ToList()));
        }
    }
}
