using System.Text.Json;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Service to extract speed values from JSON data
    /// </summary>
    public class SpeedExtractor : ISpeedExtractor
    {
        /// <summary>
        /// Extracts speed value from JSON element which can be a number or object with number/condition
        /// </summary>
        /// <param name="speedElement">JSON element representing speed (can be number or object)</param>
        /// <returns>Speed value or null if not present</returns>
        public int? ExtractSpeedValue(JsonElement? speedElement)
        {
            if (speedElement == null || !speedElement.HasValue)
                return null;

            var element = speedElement.Value;

            if (element.ValueKind == JsonValueKind.Number)
            {
                return element.GetInt32();
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                // If it's an object, look for a "number" property
                if (element.TryGetProperty("number", out var numberProperty))
                {
                    return numberProperty.GetInt32();
                }
            }

            return null;
        }
    }
}