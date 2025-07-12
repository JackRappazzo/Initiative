using Initiative.Lobby.Core.Dtos;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.SetStateTests
{
    public class GivenNewLobby : WhenTestingSetState
    {
        protected EncounterDto RetrievedState;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(EncounterDtoIsSet)
            .When(SetStateIsCalled)
            .Then(ShouldCreateLobbyWithCorrectState);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "NEWLOBBY123";
        }

        [Given]
        public void EncounterDtoIsSet()
        {
            EncounterDto = new EncounterDto
            {
                Creatures = new[] { "Orc", "Troll", "Skeleton" },
                CurrentCreatureIndex = 1,
                CurrentTurn = 3,
                CurrentMode = LobbyMode.InProgress
            };
        }

        [Then]
        public void ShouldCreateLobbyWithCorrectState()
        {
            RetrievedState = LobbyStateManager.GetState(RoomCode);
            
            Assert.That(RetrievedState, Is.Not.Null);
            Assert.That(RetrievedState.CurrentMode, Is.EqualTo(EncounterDto.CurrentMode));
            Assert.That(RetrievedState.CurrentCreatureIndex, Is.EqualTo(EncounterDto.CurrentCreatureIndex));
            Assert.That(RetrievedState.CurrentTurn, Is.EqualTo(EncounterDto.CurrentTurn));
            Assert.That(RetrievedState.Creatures, Is.EquivalentTo(EncounterDto.Creatures));
        }
    }
}