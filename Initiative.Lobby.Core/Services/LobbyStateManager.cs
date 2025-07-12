using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Initiative.Lobby.Core.Dtos;

namespace Initiative.Lobby.Core.Services
{
    public class LobbyStateManager : ILobbyStateManager
    {
        private readonly ConcurrentDictionary<string, LobbyState> lobbies = new();
        private readonly object lobbyStateLock = new();

        public EncounterDto GetState(string roomCode)
        {
            lock (lobbyStateLock)
            {
                if (!lobbies.ContainsKey(roomCode))
                {
                    lobbies[roomCode] = new LobbyState();
                }

                var lobby = lobbies[roomCode];
                return new EncounterDto
                {
                    Creatures = lobby.Creatures,
                    CurrentCreatureIndex = lobby.CurrentCreatureIndex,
                    CurrentTurn = lobby.CurrentTurn,
                    CurrentMode = lobby.CurrentMode
                };
            }
        }

        public void SetState(string roomCode, EncounterDto encounter)
        {
            lock (lobbyStateLock)
            {
                if (!lobbies.ContainsKey(roomCode))
                {
                    lobbies[roomCode] = new LobbyState();
                }

                var lobby = lobbies[roomCode];
                lobby.Creatures = encounter.Creatures.ToImmutableList();
                lobby.CurrentCreatureIndex = encounter.CurrentCreatureIndex;
                lobby.CurrentTurn = encounter.CurrentTurn;
                lobby.CurrentMode = encounter.CurrentMode;
            }
        }

        public void RemoveLobby(string roomCode)
        {
            lobbies.TryRemove(roomCode, out _);
        }

        public bool LobbyExists(string roomCode)
        {
            return lobbies.ContainsKey(roomCode);
        }

        public LobbyState GetLobbyState(string roomCode)
        {
            lock (lobbyStateLock)
            {
                if (!lobbies.ContainsKey(roomCode))
                {
                    lobbies[roomCode] = new LobbyState();
                }
                return lobbies[roomCode];
            }
        }

        public void AddConnectionToLobby(string roomCode, string connectionId)
        {
            lock (lobbyStateLock)
            {
                if (!lobbies.ContainsKey(roomCode))
                {
                    lobbies[roomCode] = new LobbyState();
                }
                
                var lobby = lobbies[roomCode];
                lobby.AddConnection(connectionId);
            }
        }

        public void RemoveConnectionFromLobby(string roomCode, string connectionId)
        {
            lock (lobbyStateLock)
            {
                if (lobbies.ContainsKey(roomCode))
                {
                    var lobby = lobbies[roomCode];
                    lobby.RemoveConnection(connectionId);
                    
                    // If no connections remain, remove the lobby
                    if (lobby.GetConnections().Count() == 0)
                    {
                        lobbies.TryRemove(roomCode, out _);
                    }
                }
            }
        }
    }
}