using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Dtos;
using Initiative.Persistence.Repositories;

namespace Initiative.Lobby.Core.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly ConcurrentDictionary<string, string> connectionToHostMap = new();
        private readonly ConcurrentDictionary<string, LobbyState> lobbies = new();
        private readonly object lobbyConnectionsLock = new();

        private readonly IInitiativeUserRepository initiativeUserRepository;

        public LobbyService(IInitiativeUserRepository initiativeUserRepository)
        {
            this.initiativeUserRepository = initiativeUserRepository;
        }

        public async Task<(bool success, LobbyServiceError error)> JoinLobby(string connectionId, string roomCode, CancellationToken cancellationToken)
        {
            if (await initiativeUserRepository.RoomCodeExists(roomCode, cancellationToken))
            {
                return (false, LobbyServiceError.RoomNotFound);
            }

            lock (lobbyConnectionsLock)
            {
                if (!lobbies.ContainsKey(roomCode))
                {
                    lobbies[roomCode] = new LobbyState();
                }
                lobbies[roomCode].AddConnection(connectionId);
            }
            connectionToHostMap[connectionId] = roomCode;

            return (true, LobbyServiceError.None);
        }

        public void LeaveLobby(string connectionId)
        {
            if (connectionToHostMap.TryRemove(connectionId, out string roomCode))
            {
                lock (lobbyConnectionsLock)
                {
                    if (lobbies.TryGetValue(roomCode, out var lobby))
                    {
                        lobby.RemoveConnection(connectionId);
                        if (lobby.GetConnections().Count() == 0)
                        {
                            lobbies.TryRemove(roomCode, out _);
                        }
                    }
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

        public void SetLobbyMode(string roomCode, LobbyMode mode)
        {
            if (lobbies.TryGetValue(roomCode, out var lobby))
            {
                lobby.CurrentMode = mode;
            }
        }

        public LobbyMode GetLobbyMode(string roomCode)
        {
            if (lobbies.TryGetValue(roomCode, out var lobby))
            {
                return lobby.CurrentMode;
            }
            return LobbyMode.Waiting;
        }

        public EncounterDto GetLobbyState(string roomCode)
        {
            if (lobbies.TryGetValue(roomCode, out var lobby))
            {
                return new EncounterDto
                {
                    Creatures = lobby.GetConnections().OrderBy(c => c),
                    CurrentCreatureIndex = lobby.CurrentCreatureIndex,
                    CurrentTurn = lobby.CurrentTurn,
                    CurrentMode = lobby.CurrentMode
                };
            }
            return new EncounterDto
            {
                Creatures = Enumerable.Empty<string>().OrderBy(c => c),
                CurrentCreatureIndex = 0,
                CurrentTurn = 0,
                CurrentMode = LobbyMode.Waiting
            };
        }

        public void SetLobbyState(string roomCode, EncounterDto encounter)
        {
            if (lobbies.TryGetValue(roomCode, out var lobby))
            {
                lobby.Creatures = encounter.Creatures.ToImmutableList();
                lobby.CurrentCreatureIndex = encounter.CurrentCreatureIndex;
                lobby.CurrentTurn = encounter.CurrentTurn;
                lobby.CurrentMode = encounter.CurrentMode;
            }
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
