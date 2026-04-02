using System.Text.Json;
using Initiative.Persistence.Models.Spell;
using MongoDB.Bson;

namespace Import.Bestiaries.Core.Parsing
{
    public class SpellParser : ISpellParser
    {
        private static readonly Dictionary<string, string> SchoolMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "A", "Abjuration" },
            { "C", "Conjuration" },
            { "D", "Divination" },
            { "E", "Enchantment" },
            { "I", "Illusion" },
            { "N", "Necromancy" },
            { "T", "Transmutation" },
            { "V", "Evocation" },
        };

        /// <summary>
        /// Parses the "spell" array from a 5etools spell JSON stream into a list of
        /// <see cref="SpellDocument"/> ready for upsert into MongoDB.
        /// </summary>
        public List<SpellDocument> Parse(string spellSourceId, string source, Stream jsonStream)
        {
            using var document = JsonDocument.Parse(jsonStream);

            if (!document.RootElement.TryGetProperty("spell", out var spellArray)
                || spellArray.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException("JSON does not contain a 'spell' array at the root.");
            }

            var spells = new List<SpellDocument>();

            foreach (var element in spellArray.EnumerateArray())
            {
                var name = element.GetProperty("name").GetString()
                    ?? throw new InvalidOperationException("Spell is missing 'name'.");

                var school = ParseSchool(element);
                var rawData = BsonDocument.Parse(element.GetRawText());

                spells.Add(new SpellDocument
                {
                    SpellSourceId = spellSourceId,
                    Source = source,
                    Name = name,
                    School = school,
                    RawData = rawData
                });
            }

            return spells;
        }

        private static string? ParseSchool(JsonElement element)
        {
            if (!element.TryGetProperty("school", out var schoolToken)
                || schoolToken.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            var code = schoolToken.GetString();
            if (code is not null && SchoolMap.TryGetValue(code, out var fullName))
                return fullName;

            return code;
        }
    }
}
