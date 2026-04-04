using System.Text.Json;
using Initiative.Persistence.Models.Condition;
using MongoDB.Bson;

namespace Import.Bestiaries.Core.Parsing
{
    public class ConditionParser : IConditionParser
    {
        /// <summary>
        /// Parses both "condition" and "status" arrays from a 5etools conditions/diseases JSON stream.
        /// Filters to only include XPHB sources (5.5 rules).
        /// </summary>
        public List<ConditionDocument> Parse(string source, Stream jsonStream)
        {
            using var document = JsonDocument.Parse(jsonStream);

            var results = new List<ConditionDocument>();

            // Parse conditions
            if (document.RootElement.TryGetProperty("condition", out var conditionArray)
                && conditionArray.ValueKind == JsonValueKind.Array)
            {
                results.AddRange(ParseArray(conditionArray, "condition"));
            }

            // Parse statuses
            if (document.RootElement.TryGetProperty("status", out var statusArray)
                && statusArray.ValueKind == JsonValueKind.Array)
            {
                results.AddRange(ParseArray(statusArray, "status"));
            }

            return results;
        }

        private static List<ConditionDocument> ParseArray(JsonElement array, string type)
        {
            var results = new List<ConditionDocument>();

            foreach (var element in array.EnumerateArray())
            {
                var name = element.GetProperty("name").GetString()
                    ?? throw new InvalidOperationException($"{type} entry is missing 'name'.");

                // Only include XPHB sources (5.5 rules)
                var entrySource = element.GetProperty("source").GetString()
                    ?? throw new InvalidOperationException($"{type} entry '{name}' is missing 'source'.");

                if (entrySource != "XPHB")
                {
                    continue;
                }

                var rawData = BsonDocument.Parse(element.GetRawText());

                results.Add(new ConditionDocument
                {
                    Source = entrySource,
                    Name = name,
                    Type = type,
                    RawData = rawData
                });
            }

            return results;
        }
    }
}
