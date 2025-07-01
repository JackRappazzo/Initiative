using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;

namespace Initiative.Lobby.Core.Dtos
{
    public class EncounterDto
    {
        public IOrderedEnumerable<string> Creatures { get; set; }
        public int CurrentCreatureIndex { get; set; } = 0;
        public int CurrentTurn { get; set; } = 0;
        public LobbyMode CurrentMode { get; set; } = LobbyMode.Waiting;
    }
}
