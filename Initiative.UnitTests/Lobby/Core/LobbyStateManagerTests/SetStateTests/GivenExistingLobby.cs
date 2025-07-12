using Initiative.Lobby.Core.Dtos;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.SetStateTests
{
    public class GivenExistingLobby : WhenTestingSetState
    {
        protected EncounterDto InitialState;
        protected EncounterDto RetrievedState;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(LobbyAlreadyExists)
            .And(UpdatedEncounterDtoIsSet)
            .When(SetStateIsCalled)
            .Then(ShouldUpdateLobbyState);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "EXISTING123";
        }

        [Given]
        public void LobbyAlreadyExists()
        {
            InitialState = new EncounterDto
            {
                Creatures = new[] { "Kobold" },
                CurrentCreatureIndex = 0,
                CurrentTurn = 1,
                CurrentMode = LobbyMode.Waiting
            };
            LobbyStateManager.SetState(RoomCode, InitialState);
        }

        [Given]
        public void UpdatedEncounterDtoIsSet()
        {
            EncounterDto = new EncounterDto
            {
                Creatures = new[] { "Ancient Dragon", "Lich", "Demon Lord", "Balor" },
                CurrentCreatureIndex = 3,
                CurrentTurn = 10,
                CurrentMode = LobbyMode.Finished
            };
        }

        [Then]
        public void ShouldUpdateLobbyState()
        {
            RetrievedState = LobbyStateManager.GetState(RoomCode);
            
            Assert.That(RetrievedState, Is.Not.Null);
            Assert.That(RetrievedState.CurrentMode, Is.EqualTo(EncounterDto.CurrentMode));
            Assert.That(RetrievedState.CurrentCreatureIndex, Is.EqualTo(EncounterDto.CurrentCreatureIndex));
            Assert.That(RetrievedState.CurrentTurn, Is.EqualTo(EncounterDto.CurrentTurn));
            Assert.That(RetrievedState.Creatures, Is.EquivalentTo(EncounterDto.Creatures));
            
            // Verify it's different from initial state
            Assert.That(RetrievedState.CurrentMode, Is.Not.EqualTo(InitialState.CurrentMode));
            Assert.That(RetrievedState.CurrentCreatureIndex, Is.Not.EqualTo(InitialState.CurrentCreatureIndex));
            Assert.That(RetrievedState.CurrentTurn, Is.Not.EqualTo(InitialState.CurrentTurn));
            Assert.That(RetrievedState.Creatures, Is.Not.EquivalentTo(InitialState.Creatures));
        }
    }
}