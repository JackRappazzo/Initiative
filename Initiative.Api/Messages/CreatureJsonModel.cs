using Newtonsoft.Json;

namespace Initiative.Api.Messages
{
    [JsonObject]
    public class CreatureJsonModel
    {
        public required string Name { get; set; }
        public int Initiative { get; set; }
        public int InitiativeModifier { get; set; } 
        public int HitPoints { get; set; }

        [JsonProperty("maxHitPoints")]
        public int MaximumHitPoints { get; set; }
        public int ArmorClass { get; set; }
    }
}
