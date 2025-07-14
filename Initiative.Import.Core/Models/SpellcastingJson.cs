using System.Text.Json.Serialization;

namespace Initiative.Import.Core.Models
{
    public class SpellcastingJson
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("headerEntries")]
        public List<string>? HeaderEntries { get; set; }

        [JsonPropertyName("will")]
        public List<string>? AtWill { get; set; }

        [JsonPropertyName("daily")]
        public Dictionary<string, List<string>>? Daily { get; set; }

        [JsonPropertyName("ability")]
        public string? SpellcastingAbility { get; set; }

        [JsonPropertyName("displayAs")]
        public string? DisplayAs { get; set; }
    }
}