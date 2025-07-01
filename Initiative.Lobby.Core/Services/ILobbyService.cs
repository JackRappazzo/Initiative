
namespace Initiative.Lobby.Core.Services
{
    public interface ILobbyService
    {
        string GetRoomCodeByConnection(string connectionId);
        Task<(bool success, LobbyServiceError error)> JoinLobby(string connectionId, string roomCode, CancellationToken cancellationToken);
        void LeaveLobby(string connectionId);
    }
}