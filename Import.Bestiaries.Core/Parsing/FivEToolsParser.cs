using System.Text.Json;
using Initiative.Persistence.Models.Bestiary;
using MongoDB.Bson;

namespace Import.Bestiaries.Core.Parsing
{
    public class FivEToolsParser : IFivEToolsParser
    {
        /// <summary>
        /// Parses the "monster" array from a 5etools bestiary JSON stream into a list of
        /// <see cref="BestiaryCreatureDocument"/> ready for upsert into MongoDB.
        /// </summary>
        public List<BestiaryCreatureDocument> Parse(string bestiaryId, string source, Stream jsonStream)
        {
            using var document = JsonDocument.Parse(jsonStream);

            if (!document.RootElement.TryGetProperty("monster", out var monsterArray)
                || monsterArray.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException("JSON does not contain a 'monster' array at the root.");
            }

            var creatures = new List<BestiaryCreatureDocument>();

            foreach (var element in monsterArray.EnumerateArray())
            {
                var name = element.GetProperty("name").GetString()
                    ?? throw new InvalidOperationException("Creature is missing 'name'.");

                var creatureType = ParseType(element);
                var challengeRating = ParseCr(element);
                var isLegendary = element.TryGetProperty("legendary", out _);
                var rawData = BsonDocument.Parse(element.GetRawText());

                creatures.Add(new BestiaryCreatureDocument
                {
                    BestiaryId = bestiaryId,
                    Source = source,
                    Name = name,
                    CreatureType = creatureType,
                    ChallengeRating = challengeRating,
                    IsLegendary = isLegendary,
                    RawData = rawData
                });
            }

            return creatures;
        }

        private static string? ParseType(JsonElement element)
        {
            if (!element.TryGetProperty("type", out var typeToken))
                return null;

            return typeToken.ValueKind switch
            {
                JsonValueKind.String => typeToken.GetString(),
                JsonValueKind.Object when typeToken.TryGetProperty("type", out var inner)
                    => inner.ValueKind == JsonValueKind.String ? inner.GetString() : null,
                _ => null
            };
        }

        private static string? ParseCr(JsonElement element)
        {
            if (!element.TryGetProperty("cr", out var crToken))
                return null;

            return crToken.ValueKind switch
            {
                JsonValueKind.String => crToken.GetString(),
                JsonValueKind.Object when crToken.TryGetProperty("cr", out var inner) => inner.GetString(),
                _ => null
            };
        }
    }
}
