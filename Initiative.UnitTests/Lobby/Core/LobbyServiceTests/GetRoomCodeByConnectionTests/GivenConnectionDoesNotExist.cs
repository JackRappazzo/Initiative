using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetRoomCodeByConnectionTests
{
    public class GivenConnectionDoesNotExist : WhenTestingGetRoomCodeByConnection
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NonExistentConnectionIdIsSet)
            .When(GetRoomCodeByConnectionIsCalled)
            .Then(ShouldReturnEmptyString);

        [Given]
        public void NonExistentConnectionIdIsSet()
        {
            ConnectionId = "NONEXISTENT_CONNECTION";
        }

        [Then]
        public void ShouldReturnEmptyString()
        {
            Assert.That(Result, Is.Empty);
        }
    }
}
