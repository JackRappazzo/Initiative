using System.Text.Json.Serialization;

namespace Initiative.Import.Core.Models
{
    /// <summary>
    /// Root model for the monster manual JSON file
    /// </summary>
    public class MonsterManualJson
    {
        [JsonPropertyName("monster")]
        public List<MonsterJson> Monsters { get; set; } = new();
    }
}