using System.Collections.Immutable;
using Initiative.Lobby.Core.Dtos;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.GetStateTests
{
    public class GivenLobbyExistsWithState : WhenTestingGetState
    {
        protected EncounterDto InitialState;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RoomCodeIsSet)
            .And(LobbyStateIsSet)
            .When(GetStateIsCalled)
            .Then(ShouldReturnCorrectState);

        [Given]
        public void RoomCodeIsSet()
        {
            RoomCode = "EXISTING123";
        }

        [Given]
        public void LobbyStateIsSet()
        {
            InitialState = new EncounterDto
            {
                Creatures = new[] { "Dragon", "Goblin", "Wizard" },
                CurrentCreatureIndex = 2,
                CurrentTurn = 5,
                CurrentMode = LobbyMode.InProgress
            };
            LobbyStateManager.SetState(RoomCode, InitialState);
        }

        [Then]
        public void ShouldReturnCorrectState()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.CurrentMode, Is.EqualTo(InitialState.CurrentMode));
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(InitialState.CurrentCreatureIndex));
            Assert.That(Result.CurrentTurn, Is.EqualTo(InitialState.CurrentTurn));
            Assert.That(Result.Creatures, Is.EquivalentTo(InitialState.Creatures));
        }
    }
}