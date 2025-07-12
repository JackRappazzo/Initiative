using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Initiative.Lobby.Core.Dtos;

namespace Initiative.Lobby.Core.Services
{
    public class LobbyStateManager
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
    }
}