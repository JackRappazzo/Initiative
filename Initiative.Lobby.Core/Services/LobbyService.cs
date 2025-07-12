using Initiative.Lobby.Core.Dtos;
using Initiative.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Lobby.Core.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly ConcurrentDictionary<string, string> connectionToHostMap = new();
        private readonly object lobbyConnectionsLock = new();

        private readonly IServiceScopeFactory serviceScopeFactory;

        public LobbyService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<(bool success, LobbyServiceError error)> JoinLobby(string connectionId, string roomCode, CancellationToken cancellationToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var initiativeUserRepository = scope.ServiceProvider.GetRequiredService<IInitiativeUserRepository>();
            var lobbyStateManager = scope.ServiceProvider.GetRequiredService<ILobbyStateManager>();

            if (!await initiativeUserRepository.RoomCodeExists(roomCode, cancellationToken))
            {
                return (false, LobbyServiceError.RoomNotFound);
            }

            lock (lobbyConnectionsLock)
            {
                lobbyStateManager.AddConnectionToLobby(roomCode, connectionId);
                connectionToHostMap[connectionId] = roomCode;
            }

            return (true, LobbyServiceError.None);
        }

        public void LeaveLobby(string connectionId)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var lobbyStateManager = scope.ServiceProvider.GetRequiredService<ILobbyStateManager>();
            
            if (connectionToHostMap.TryRemove(connectionId, out string roomCode))
            {
                lock (lobbyConnectionsLock)
                {
                    lobbyStateManager.RemoveConnectionFromLobby(roomCode, connectionId);
                }
            }
        }

        public string GetRoomCodeByConnection(string connectionId)
        {
            if (connectionToHostMap.TryGetValue(connectionId, out string roomCode))
            {
                return roomCode;
            }
            return string.Empty;
        }

        public async Task<EncounterDto> GetLobbyState(string roomCode)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var lobbyStateManager = scope.ServiceProvider.GetRequiredService<ILobbyStateManager>();
            if (lobbyStateManager.LobbyExists(roomCode))
            {
                return lobbyStateManager.GetState(roomCode);
            }
            else
            {
                var lobbyStateRepository = scope
                    .ServiceProvider
                    .GetRequiredService<ILobbyStateRepository>();

                var storedState = await lobbyStateRepository.FetchLobbyStateByRoomCode(roomCode, CancellationToken.None);
                if (storedState != null)
                {
                    var recoveredEncounterDto = new EncounterDto
                    {
                        Creatures = storedState.Creatures.ToImmutableList(),
                        CurrentCreatureIndex = storedState.CurrentCreatureIndex,
                        CurrentTurn = storedState.TurnNumber,
                        CurrentMode = storedState.CurrentMode
                    };

                    await SetLobbyState(roomCode, recoveredEncounterDto);
                    return recoveredEncounterDto;
                }
                else
                {
                    return new EncounterDto
                    {
                        Creatures = Enumerable.Empty<string>(),
                        CurrentCreatureIndex = 0,
                        CurrentTurn = 0,
                        CurrentMode = LobbyMode.Waiting
                    };
                }
            }
        }

        public async Task SetLobbyState(string roomCode, EncounterDto encounter)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var lobbyStateManager = scope.ServiceProvider.GetRequiredService<ILobbyStateManager>();
            lobbyStateManager.SetState(roomCode, encounter);

            var lobbyStateRepository = serviceScopeFactory
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ILobbyStateRepository>();

            await lobbyStateRepository.UpsertLobbyState(
                roomCode,
                encounter.Creatures.ToArray(),
                encounter.CurrentTurn,
                encounter.CurrentCreatureIndex,
                encounter.CurrentMode,
                CancellationToken.None);
        }
    }

    public enum LobbyServiceError
    {
        None,
        RoomNotFound,
        UserAlreadyInRoom,
        UserNotInRoom,
        InvalidOperation
    }
}
