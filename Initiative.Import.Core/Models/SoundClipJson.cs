using System.Text.Json.Serialization;

namespace Initiative.Import.Core.Models
{
    public class SoundClipJson
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;
    }
}