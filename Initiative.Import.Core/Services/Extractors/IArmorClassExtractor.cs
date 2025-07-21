using System.Text.Json;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Interface for extracting armor class values from JSON data
    /// </summary>
    public interface IArmorClassExtractor
    {
        /// <summary>
        /// Extracts armor class from JSON elements which can be a number or object
        /// </summary>
        /// <param name="armorClassList">List of JSON elements representing armor class</param>
        /// <returns>Armor class value or default if not present</returns>
        int ExtractArmorClass(List<JsonElement>? armorClassList);
    }
}