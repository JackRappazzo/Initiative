using System.Text.Json.Serialization;

namespace Initiative.Import.Core.Models
{
    public class HitPointsJson
    {
        [JsonPropertyName("average")]
        public int Average { get; set; }

        [JsonPropertyName("formula")]
        public string Formula { get; set; } = string.Empty;
    }
}