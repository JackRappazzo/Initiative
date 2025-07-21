using System.Text.Json;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Service to extract condition immunities from JSON data
    /// </summary>
    public class ConditionImmunitiesExtractor : IConditionImmunitiesExtractor
    {
        /// <summary>
        /// Extracts condition immunities from JSON elements which can be strings or objects
        /// </summary>
        /// <param name="conditionImmunitiesList">List of JSON elements representing condition immunities</param>
        /// <returns>List of condition immunity names</returns>
        public List<string> ExtractConditionImmunities(List<JsonElement>? conditionImmunitiesList)
        {
            if (conditionImmunitiesList == null || !conditionImmunitiesList.Any())
                return new List<string>();

            var conditions = new List<string>();

            foreach (var element in conditionImmunitiesList)
            {
                if (element.ValueKind == JsonValueKind.String)
                {
                    // Simple string condition
                    conditions.Add(element.GetString() ?? string.Empty);
                }
                else if (element.ValueKind == JsonValueKind.Object)
                {
                    // Complex object with conditionImmune array
                    if (element.TryGetProperty("conditionImmune", out var conditionProperty) &&
                        conditionProperty.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var condition in conditionProperty.EnumerateArray())
                        {
                            if (condition.ValueKind == JsonValueKind.String)
                            {
                                conditions.Add(condition.GetString() ?? string.Empty);
                            }
                        }
                    }
                }
            }

            return conditions.Where(c => !string.IsNullOrEmpty(c)).ToList();
        }
    }
}