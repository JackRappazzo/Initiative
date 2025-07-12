using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Lobby.Core.LobbyStateManagerTests.RemoveConnectionFromLobbyTests
{
    public abstract class WhenTestingRemoveConnectionFromLobby : WhenTestingLobbyStateManager
    {
        protected string ConnectionId;

        [When]
        public void RemoveConnectionFromLobbyIsCalled()
        {
            LobbyStateManager.RemoveConnectionFromLobby(RoomCode, ConnectionId);
        }
    }
}