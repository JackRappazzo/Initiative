using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.AddConnectionToLobbyTests
{
    public abstract class WhenTestingAddConnectionToLobby : WhenTestingLobbyStateManager
    {
        protected string ConnectionId;

        [When]
        public void AddConnectionToLobbyIsCalled()
        {
            LobbyStateManager.AddConnectionToLobby(RoomCode, ConnectionId);
        }
    }
}