using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetLobbyStateTests
{
    public class GivenLobbyDoesNotExist : WhenTestingGetLobbyState
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NonExistentRoomCodeIsSet)
            .When(GetLobbyStateIsCalled)
            .Then(ShouldReturnEmptyState);

        [Given]
        public void NonExistentRoomCodeIsSet()
        {
            RoomCode = "NONEXISTENT";
        }

        [Then]
        public void ShouldReturnEmptyState()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Creatures, Is.Empty);
            Assert.That(Result.CurrentCreatureIndex, Is.EqualTo(0));
            Assert.That(Result.CurrentTurn, Is.EqualTo(0));
            Assert.That(Result.CurrentMode, Is.EqualTo(LobbyMode.Waiting));
        }
    }
}
