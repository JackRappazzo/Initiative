using System.Text.Json;
using System.Text.Json.Serialization;

namespace Initiative.Import.Core.Models
{
    public class SpeedJson
    {
        [JsonPropertyName("walk")]
        public JsonElement? Walk { get; set; } // Can be number or object with condition

        [JsonPropertyName("fly")]
        public JsonElement? Fly { get; set; } // Can be number or object with condition

        [JsonPropertyName("swim")]
        public JsonElement? Swim { get; set; } // Can be number or object with condition

        [JsonPropertyName("burrow")]
        public JsonElement? Burrow { get; set; } // Can be number or object with condition

        [JsonPropertyName("climb")]
        public JsonElement? Climb { get; set; } // Can be number or object with condition

        [JsonPropertyName("canHover")]
        public bool? CanHover { get; set; }
    }
}