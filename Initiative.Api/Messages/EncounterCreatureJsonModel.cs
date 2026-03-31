using Newtonsoft.Json;

namespace Initiative.Api.Messages
{
    [JsonObject]
    public class EncounterCreatureJsonModel
    {
        public bool IsPlayer { get; set; }

        public required string DisplayName { get; set; }

        public string? CreatureName { get; set; }

        public string? CreatureId { get; set; }

        public int Initiative { get; set; }

        public int MaxHP { get; set; }

        public int CurrentHP { get; set; }

        public int AC { get; set; }
    }
}
