using Initiative.Lobby.Core.Dtos;

namespace Initiative.Lobby.Core.Services
{
    public interface ILobbyStateManager
    {
        EncounterDto GetState(string roomCode);
        void SetState(string roomCode, EncounterDto encounter);
        void RemoveLobby(string roomCode);
        bool LobbyExists(string roomCode);
        LobbyState GetLobbyState(string roomCode);
        void AddConnectionToLobby(string roomCode, string connectionId);
        void RemoveConnectionFromLobby(string roomCode, string connectionId);
    }
}