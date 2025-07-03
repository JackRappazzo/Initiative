using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.JoinLobbyTests
{
    public abstract class WhenTestingJoinLobby : WhenTestingLobbyService
    {
        protected string ConnectionId;
        protected string RoomCode;
        protected (bool success, LobbyServiceError error) Result;

        [When]
        public async Task JoinLobbyIsCalled()
        {
            Result = await LobbyService.JoinLobby(ConnectionId, RoomCode, CancellationToken);
        }
    }
}
