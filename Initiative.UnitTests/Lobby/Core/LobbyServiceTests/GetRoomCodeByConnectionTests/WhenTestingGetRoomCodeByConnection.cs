using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyServiceTests.GetRoomCodeByConnectionTests
{
    public abstract class WhenTestingGetRoomCodeByConnection : WhenTestingLobbyService
    {
        protected string ConnectionId;
        protected string Result;

        [When]
        public void GetRoomCodeByConnectionIsCalled()
        {
            Result = LobbyService.GetRoomCodeByConnection(ConnectionId);
        }
    }
}
