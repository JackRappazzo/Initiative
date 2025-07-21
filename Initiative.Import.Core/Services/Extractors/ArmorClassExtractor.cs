using System.Text.Json;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Service to extract armor class values from JSON data
    /// </summary>
    public class ArmorClassExtractor : IArmorClassExtractor
    {
        /// <summary>
        /// Extracts armor class from JSON elements which can be a number or object
        /// </summary>
        /// <param name="armorClassList">List of JSON elements representing armor class</param>
        /// <returns>Armor class value or default if not present</returns>
        public int ExtractArmorClass(List<JsonElement>? armorClassList)
        {
            if (armorClassList == null || !armorClassList.Any())
                return 10; // Default AC

            var firstElement = armorClassList.First();

            if (firstElement.ValueKind == JsonValueKind.Number)
            {
                return firstElement.GetInt32();
            }
            else if (firstElement.ValueKind == JsonValueKind.Object)
            {
                // If it's an object, look for an "ac" property
                if (firstElement.TryGetProperty("ac", out var acProperty))
                {
                    return acProperty.GetInt32();
                }
            }

            return 10; // Default fallback
        }
    }
}