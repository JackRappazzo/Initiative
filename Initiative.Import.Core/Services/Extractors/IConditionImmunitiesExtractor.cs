using System.Text.Json;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Interface for extracting condition immunities from JSON data
    /// </summary>
    public interface IConditionImmunitiesExtractor
    {
        /// <summary>
        /// Extracts condition immunities from JSON elements which can be strings or objects
        /// </summary>
        /// <param name="conditionImmunitiesList">List of JSON elements representing condition immunities</param>
        /// <returns>List of condition immunity names</returns>
        List<string> ExtractConditionImmunities(List<JsonElement>? conditionImmunitiesList);
    }
}