using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.LeaveLobbyTests
{
    public abstract class WhenTestingLeaveLobby : WhenTestingLobbyService
    {
        protected string ConnectionId;
        protected string RoomCode;

        [When]
        public void LeaveLobbyIsCalled()
        {
            LobbyService.LeaveLobby(ConnectionId);
        }
    }
}