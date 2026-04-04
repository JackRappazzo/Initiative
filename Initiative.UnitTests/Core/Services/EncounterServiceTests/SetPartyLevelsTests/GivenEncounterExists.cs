using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.SetPartyLevelsTests
{
    public class GivenEncounterExists : WhenTestingSetPartyLevels
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterIdIsSet)
            .And(OwnerIdIsSet)
            .And(PartyLevelsAreSet)
            .And(RepositoryReturnsEncounter)
            .When(SetPartyLevelsIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldPersistSanitizedPartyLevels);

        [Given]
        public void EncounterIdIsSet()
        {
            EncounterId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public void PartyLevelsAreSet()
        {
            PartyLevels = new[] { 0, 5, 25 };
        }

        [Given]
        public void RepositoryReturnsEncounter()
        {
            EncounterRepository.FetchEncounterById(EncounterId, OwnerId, CancellationToken)
                .Returns(new Encounter
                {
                    Id = EncounterId,
                    OwnerId = OwnerId,
                    DisplayName = "Test Encounter",
                    Creatures = new List<EncounterCreature>()
                });
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldPersistSanitizedPartyLevels()
        {
            EncounterRepository.Received(1).SetEncounterPartyLevels(
                EncounterId,
                Arg.Is<IEnumerable<int>>(levels => levels.SequenceEqual(new[] { 1, 5, 20 })),
                CancellationToken);
        }
    }
}
