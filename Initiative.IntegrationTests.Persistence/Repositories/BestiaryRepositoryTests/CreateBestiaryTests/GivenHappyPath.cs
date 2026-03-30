using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.CreateBestiaryTests
{
    public class GivenHappyPath : WhenTestingCreateBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryIsSet)
            .When(CreateBestiaryIsCalled)
            .Then(ShouldReturnId)
            .And(ShouldPersistBestiary);

        [Given]
        public void BestiaryIsSet()
        {
            Bestiary = BuildBestiary(name: "Monster Manual", source: "MM");
        }

        [Then]
        public void ShouldReturnId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
        }

        [Then]
        public async Task ShouldPersistBestiary()
        {
            var stored = await BestiaryRepository.GetBestiaryBySource("MM", CancellationToken);

            Assert.That(stored, Is.Not.Null);
            Assert.That(stored!.Id, Is.EqualTo(Result));
            Assert.That(stored.Name, Is.EqualTo("Monster Manual"));
            Assert.That(stored.Source, Is.EqualTo("MM"));
            Assert.That(stored.OwnerId, Is.Null);
        }
    }
}
