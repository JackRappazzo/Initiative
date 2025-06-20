using Microsoft.AspNetCore.SignalR;

namespace Initiative.Lobby.Core
{
    public class LobbyHub : Hub
    {

        public async Task SendNextTurn(string sender)
        {
            await Clients.All.SendAsync("NextTurn", sender);
        }

        public async Task SendCreatureList(string sender, List<string> creatureList)
        {
            await Clients.All.SendAsync("InitiativeList", sender, creatureList);
        }

        public async Task JoinLobby(string lobbyName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
            await Clients.Group(lobbyName).SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task LeaveLobby(string lobbyName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyName);
            await Clients.Group(lobbyName).SendAsync("UserLeft", Context.ConnectionId);
        }

    }
}
