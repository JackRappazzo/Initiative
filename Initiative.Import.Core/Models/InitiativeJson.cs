using System.Text.Json.Serialization;

namespace Initiative.Import.Core.Models
{
    public class InitiativeJson
    {
        [JsonPropertyName("proficiency")]
        public int? Proficiency { get; set; }
    }
}