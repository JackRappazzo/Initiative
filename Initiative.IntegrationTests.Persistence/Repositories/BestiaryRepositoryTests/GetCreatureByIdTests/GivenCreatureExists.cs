using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.GetCreatureByIdTests
{
    public class GivenCreatureExists : WhenTestingGetCreatureById
    {
        private string _bestiaryId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAndCreatureExist)
            .When(GetCreatureByIdIsCalled)
            .Then(ShouldReturnCreature);

        [Given]
        public async Task BestiaryAndCreatureExist()
        {
            _bestiaryId = await BestiaryRepository.CreateBestiary(BuildBestiary(source: "GC1"), CancellationToken);

            await BestiaryRepository.UpsertCreatures(
                [BuildCreature(_bestiaryId, name: "Troll", creatureType: "giant", cr: "5")],
                CancellationToken);

            // Retrieve the ID that was assigned during upsert
            var inserted = await BestiaryRepository.SearchCreatures(
                new Initiative.Persistence.Models.Bestiary.BestiarySearchQuery { BestiaryIds = [_bestiaryId] },
                CancellationToken);

            CreatureId = inserted.Single().Id;
        }

        [Then]
        public void ShouldReturnCreature()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result!.Name, Is.EqualTo("Troll"));
            Assert.That(Result.CreatureType, Is.EqualTo("giant"));
            Assert.That(Result.ChallengeRating, Is.EqualTo("5"));
            Assert.That(Result.RawData, Is.Not.Null);
        }
    }
}
