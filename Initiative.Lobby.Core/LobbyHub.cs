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

        public async Task SendNextTurn()
        {
            var connectionId = Context.ConnectionId;
            var lobby = lobbyService.GetRoomCodeByConnection(connectionId);
            await Clients.Group(lobby).SendAsync("NextTurn");
        }

        public async Task SendCreatureList(List<string> creatureList)
        {
            var lobby = lobbyService.GetRoomCodeByConnection(Context.ConnectionId);
            if(string.IsNullOrEmpty(lobby))
            {
                await Clients.Caller.SendAsync("Error", "You are not in a lobby.");
                return;
            }
            await Clients.Group(lobby).SendAsync("CreatureList", creatureList);
        }

        public async Task JoinLobby(string roomCode)
        {
            var (success, error) = await lobbyService.JoinLobby(Context.ConnectionId, roomCode, Context.ConnectionAborted);
            if (success)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
                await Clients.Group(roomCode).SendAsync("UserJoined", Context.ConnectionId);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", $"Failed to join lobby: {error}");
            }
        }

        public async Task StartEncounter(IEnumerable<string> creatures)
        {
            var lobby = lobbyService.GetRoomCodeByConnection(Context.ConnectionId);
            if (string.IsNullOrEmpty(lobby))
            {
                await Clients.Caller.SendAsync("Error", "You are not in a lobby.");
                return;
            }

            lobbyService.SetLobbyMode(lobby, LobbyMode.InProgress);
            await Clients.Group(lobby).SendAsync("StartEncounter", creatures);
        }

        public async Task EndEncounter()
        {
            var lobby = lobbyService.GetRoomCodeByConnection(Context.ConnectionId);
            if (string.IsNullOrEmpty(lobby))
            {
                await Clients.Caller.SendAsync("Error", "You are not in a lobby.");
                return;
            }
            lobbyService.SetLobbyMode(lobby, LobbyMode.Waiting);
            await Clients.Group(lobby).SendAsync("EndEncounter");
        }

        public async Task LeaveLobby(string roomCode)
        {
            lobbyService.LeaveLobby(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Group(roomCode).SendAsync("UserLeft", Context.ConnectionId);
        }

    }
}
