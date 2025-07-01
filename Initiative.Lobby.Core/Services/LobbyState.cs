using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Lobby.Core.Services
{
    public class LobbyState
    {

        public IEnumerable<string> Creatures { get; set; } = Enumerable.Empty<string>();
        public int CurrentCreatureIndex { get; set; } = 0;
        public int CurrentTurn { get; set; } = 0;

        public LobbyMode CurrentMode { get; set; } = LobbyMode.Waiting;

        private HashSet<string> connectionIds = new HashSet<string>();

        public LobbyState() { }

        public void AddConnection(string connectionId)
        {
            if(!connectionIds.Contains(connectionId))
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

        public IEnumerable<string> GetConnections()
        {
            return connectionIds.ToList();
        }
    }
}
