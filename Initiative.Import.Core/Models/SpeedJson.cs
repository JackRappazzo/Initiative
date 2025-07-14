using System.Text.Json;
using System.Text.Json.Serialization;

namespace Initiative.Import.Core.Models
{
    public class SpeedJson
    {
        [JsonPropertyName("walk")]
        public int? Walk { get; set; }

        [JsonPropertyName("fly")]
        public JsonElement? Fly { get; set; } // Can be number or object with condition

        [JsonPropertyName("swim")]
        public int? Swim { get; set; }

        [JsonPropertyName("burrow")]
        public int? Burrow { get; set; }

        [JsonPropertyName("climb")]
        public int? Climb { get; set; }

        [JsonPropertyName("canHover")]
        public bool? CanHover { get; set; }
    }
}