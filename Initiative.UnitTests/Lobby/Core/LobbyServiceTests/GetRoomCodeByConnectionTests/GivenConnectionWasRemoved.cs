using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetRoomCodeByConnectionTests
{
    using LeapingGorilla.Testing.Core.Attributes;
    using LeapingGorilla.Testing.Core.Composable;
    using LeapingGorilla.Testing.NUnit.Attributes;
    using NSubstitute;

    namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetRoomCodeByConnectionTests
    {
        public class GivenConnectionWasRemoved : WhenTestingGetRoomCodeByConnection
        {
            protected string RoomCode;

            protected override ComposedTest ComposeTest() => TestComposer
                .Given(ConnectionDetailsAreSet)
                .And(ConnectionIsAddedToLobby)
                .And(ConnectionIsRemoved)
                .When(GetRoomCodeByConnectionIsCalled)
                .Then(ShouldReturnEmptyString);

            [Given]
            public void ConnectionDetailsAreSet()
            {
                ConnectionId = Guid.NewGuid().ToString();
                RoomCode = "TEST123";
            }

            [Given]
            public async Task ConnectionIsAddedToLobby()
            {
                InitiativeUserRepository.RoomCodeExists(RoomCode, CancellationToken)
                    .Returns(Task.FromResult(true));

                await LobbyService.JoinLobby(ConnectionId, RoomCode, CancellationToken);
            }

            [Given]
            public void ConnectionIsRemoved()
            {
                LobbyService.LeaveLobby(ConnectionId);
            }

            [Then]
            public void ShouldReturnEmptyString()
            {
                Assert.That(Result, Is.Empty);
            }
        }
    }
}
