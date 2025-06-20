using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Lobby.Core.Services
{
    public class Lobby
    {
        public string OwnerId { get; set; }
        public string LobbyKey { get; set; }

        public IEnumerable<string> ConnectionIds {  get=> connectionIds.ToImmutableList(); }

        private List<string> connectionIds = new List<string>();

        public Lobby(string ownerId, string lobbyKey)
        {
            OwnerId = ownerId;
            LobbyKey = lobbyKey;
        }

        public void AddConnection(string connectionId)
        {
            if (!connectionIds.Contains(connectionId))
            {
                connectionIds.Add(connectionId);
            }
        }

        public void RemoveConnection(string connectionId)
        {
            if (connectionIds.Contains(connectionId))
            {
                connectionIds.Remove(connectionId);
            }
        }

    }
}
