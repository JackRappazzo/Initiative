using Initiative.Persistence.Models.Lobby;
using Initiative.Lobby.Core.Services;

namespace Initiative.Persistence.Repositories
{
    public interface ILobbyStateRepository
    {
        Task<LobbyStateDto> FetchLobbyStateByRoomCode(string roomCode, CancellationToken cancellationToken);
        Task<string> UpsertLobbyState(string roomCode, string[] creatures, int turnNumber, int currentCreatureIndex, LobbyMode currentMode, CancellationToken cancellationToken);
    }
}