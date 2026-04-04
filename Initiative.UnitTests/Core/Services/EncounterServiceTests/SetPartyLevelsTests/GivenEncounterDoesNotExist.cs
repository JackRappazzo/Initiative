using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Initiative.UnitTests.Core.Services.EncounterServiceTests.SetPartyLevelsTests
{
    public class GivenEncounterDoesNotExist : WhenTestingSetPartyLevels
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EncounterIdIsSet)
            .And(OwnerIdIsSet)
            .And(PartyLevelsAreSet)
            .And(RepositoryDoesNotFindEncounter)
            .When(SetPartyLevelsIsCalled)
            .Then(ShouldThrowException)
            .And(ShouldNotPersistPartyLevels);

        [Given]
        public void EncounterIdIsSet()
        {
            EncounterId = Guid.NewGuid().ToString();
        }

        [Given]
        public void OwnerIdIsSet()
        {
            OwnerId = Guid.NewGuid().ToString();
        }

        [Given]
        public void PartyLevelsAreSet()
        {
            PartyLevels = new[] { 5, 6, 7 };
        }

        [Given]
        public void RepositoryDoesNotFindEncounter()
        {
            EncounterRepository.FetchEncounterById(EncounterId, OwnerId, CancellationToken)
                .ReturnsNull();
        }

        [Then]
        public void ShouldThrowException()
        {
            Assert.That(ThrownException, Is.TypeOf<ArgumentException>());
        }

        [Then]
        public void ShouldNotPersistPartyLevels()
        {
            EncounterRepository.DidNotReceiveWithAnyArgs()
                .SetEncounterPartyLevels(default!, default!, default);
        }
    }
}
