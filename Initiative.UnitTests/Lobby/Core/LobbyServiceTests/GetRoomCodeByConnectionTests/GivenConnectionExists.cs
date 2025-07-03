using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetRoomCodeByConnectionTests
{
    public class GivenConnectionExists : WhenTestingGetRoomCodeByConnection
    {
        protected string ExpectedRoomCode;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ConnectionDetailsAreSet)
            .And(ConnectionIsAddedToLobby)
            .When(GetRoomCodeByConnectionIsCalled)
            .Then(ShouldReturnCorrectRoomCode);

        [Given]
        public void ConnectionDetailsAreSet()
        {
            ConnectionId = Guid.NewGuid().ToString();
            ExpectedRoomCode = "TEST123";
        }

        [Given]
        public async Task ConnectionIsAddedToLobby()
        {
            InitiativeUserRepository.RoomCodeExists(ExpectedRoomCode, CancellationToken)
                .Returns(true);

            await LobbyService.JoinLobby(ConnectionId, ExpectedRoomCode, CancellationToken);
        }

        [Then]
        public void ShouldReturnCorrectRoomCode()
        {
            Assert.That(Result, Is.EqualTo(ExpectedRoomCode));
        }
    }
}
