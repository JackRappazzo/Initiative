using Initiative.Lobby.Core.Dtos;
using Initiative.Lobby.Core.Services;
using Microsoft.AspNetCore.SignalR;

namespace Initiative.Lobby.Core
{
    public class LobbyHub : Hub
    {
        private readonly ILobbyService lobbyService;

        public LobbyHub(ILobbyService lobbyService)
        {
            this.lobbyService = lobbyService;
        }

        private async Task<string> ValidateLobbyAccess()
        {
            var lobby = lobbyService.GetRoomCodeByConnection(Context.ConnectionId);
            if (string.IsNullOrEmpty(lobby))
            {
                await Clients.Caller.SendAsync("Error", "You are not in a lobby.");
                return string.Empty;
            }
            return lobby;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var lobby = lobbyService.GetRoomCodeByConnection(Context.ConnectionId);
            if (!string.IsNullOrEmpty(lobby))
            {
                await LeaveLobby(lobby);
            }
            await base.OnDisconnectedAsync(exception);
        }

        // Lobby Management
        public async Task JoinLobby(string roomCode)
        {
            var (success, error) = await lobbyService.JoinLobby(Context.ConnectionId, roomCode, Context.ConnectionAborted);
            if (success || error == LobbyServiceError.UserAlreadyInRoom)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
                await Clients.Group(roomCode).SendAsync("UserJoined", Context.ConnectionId);
                await Clients.Caller.SendAsync("LobbyJoined", lobbyService.GetLobbyState(roomCode));
                return;
            }

            await Clients.Caller.SendAsync("Error", $"Failed to join lobby: {error}");
        }

        public async Task LeaveLobby(string roomCode)
        {
            lobbyService.LeaveLobby(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Group(roomCode).SendAsync("UserLeft", Context.ConnectionId);
        }


        public async Task GetLobbyState()
        {
            var lobby = await ValidateLobbyAccess();
            if (string.IsNullOrEmpty(lobby)) return;

            var lobbyState = lobbyService.GetLobbyState(lobby);
            await Clients.Caller.SendAsync("ReceivedLobbyState", lobbyState);
        }

        public async Task SetLobbyState(EncounterDto encounterDto)
        {
            var lobby = await ValidateLobbyAccess();
            if (string.IsNullOrEmpty(lobby)) return;

            lobbyService.SetLobbyState(lobby, encounterDto);
            await Clients.OthersInGroup(lobby).SendAsync("ReceivedLobbyState", encounterDto);
        }
    }
}