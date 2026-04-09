using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Lobby.Core.Services;
using Newtonsoft.Json;

namespace Initiative.Lobby.Core.Dtos
{
    [JsonObject]
    public class LobbyCreatureDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public IEnumerable<string> Statuses { get; set; } = Enumerable.Empty<string>();
        public string HealthStatus { get; set; } = string.Empty;
        public bool IsPlayer { get; set; }
        public bool IsHidden { get; set; }
    }

    [JsonObject]
    public class EncounterDto
    {
        public IEnumerable<LobbyCreatureDto> Creatures { get; set; } = Enumerable.Empty<LobbyCreatureDto>();
        public int CurrentCreatureIndex { get; set; } = 0;
        public int CurrentTurn { get; set; } = 0;
        public LobbyMode CurrentMode { get; set; }
    }
}
