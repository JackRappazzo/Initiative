
using Initiative.Lobby.Core.Dtos;

namespace Initiative.Lobby.Core.Services
{
    public interface ILobbyService
    {
        EncounterDto GetLobbyState(string roomCode);
        void SetLobbyState(string roomCode, EncounterDto encounter);
        string GetRoomCodeByConnection(string connectionId);
        Task<(bool success, LobbyServiceError error)> JoinLobby(string connectionId, string roomCode, CancellationToken cancellationToken);
        void LeaveLobby(string connectionId);

    }
}