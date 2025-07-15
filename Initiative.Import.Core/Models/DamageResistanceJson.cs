using System.Text.Json.Serialization;

namespace Initiative.Import.Core.Models
{
    /// <summary>
    /// Represents a damage resistance that can be either a standard damage type or a special case
    /// </summary>
    public class DamageResistanceJson
    {
        /// <summary>
        /// For special resistance cases where the actual damage type is dynamic
        /// </summary>
        [JsonPropertyName("special")]
        public string? Special { get; set; }
        
        /// <summary>
        /// The damage type when resistance is a simple string
        /// </summary>
        public DamageType? DamageType { get; set; }
        
        /// <summary>
        /// The raw string value for logging/debugging purposes
        /// </summary>
        public string? RawValue { get; set; }
        
        /// <summary>
        /// Gets the display name for this resistance
        /// </summary>
        public string GetDisplayName()
        {
            if (!string.IsNullOrEmpty(Special))
                return Special;
            
            return DamageType?.ToString() ?? RawValue ?? "Unknown";
        }
    }
}