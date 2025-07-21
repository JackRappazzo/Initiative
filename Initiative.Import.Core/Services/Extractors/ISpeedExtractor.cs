using System.Text.Json;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Interface for extracting speed values from JSON data
    /// </summary>
    public interface ISpeedExtractor
    {
        /// <summary>
        /// Extracts speed value from JSON element which can be a number or object with number/condition
        /// </summary>
        /// <param name="speedElement">JSON element representing speed (can be number or object)</param>
        /// <returns>Speed value or null if not present</returns>
        int? ExtractSpeedValue(JsonElement? speedElement);
    }
}