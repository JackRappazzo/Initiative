using System.Text.Json;
using System.Text.Json.Serialization;

namespace Initiative.Import.Core.Models
{
    public class ActionJson
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("entries")]
        public List<JsonElement> Entries { get; set; } = new();
    }
}