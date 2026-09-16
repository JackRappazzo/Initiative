using System.Text.Json;

namespace Initiative.Api.Core.Services.DndBeyond
{
    public class DndBeyondProxyService : IDndBeyondProxyService
    {
        private readonly DndBeyondClient dndBeyondClient;

        public DndBeyondProxyService(DndBeyondClient dndBeyondClient)
        {
            this.dndBeyondClient = dndBeyondClient;
        }

        public async Task<IEnumerable<DndBeyondCharacterListItem>> GetCharacters(string token, CancellationToken cancellationToken)
        {
            using var document = await dndBeyondClient.GetCharactersAsync(token, cancellationToken);

            if (!document.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            var result = new List<DndBeyondCharacterListItem>();
            foreach (var item in data.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                result.Add(new DndBeyondCharacterListItem
                {
                    Id = GetLong(item, "id"),
                    Name = GetString(item, "characterName") ?? GetString(item, "name")
                });
            }

            return result;
        }

        public async Task<DndBeyondCharacterDetail?> GetCharacter(string token, string characterId, CancellationToken cancellationToken)
        {
            using var document = await dndBeyondClient.GetCharacterAsync(token, characterId, cancellationToken);
            if (document is null)
            {
                return null;
            }

            if (!document.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var removedHp = GetInt(data, "removedHitPoints");
            var level = GetLevel(data);

            var maxHp = CalculateMaxHitPoints(data, level);
            var currentHp = Math.Max(maxHp - removedHp, 0);

            return new DndBeyondCharacterDetail
            {
                Id = GetLong(data, "id"),
                Name = GetString(data, "characterName") ?? GetString(data, "name"),
                Level = level,
                ClassName = GetClassName(data),
                MaxHP = maxHp,
                CurrentHP = currentHp,
                TemporaryHP = GetInt(data, "temporaryHitPoints")
            };
        }

        internal static int CalculateMaxHitPoints(JsonElement data, int level)
        {
            var overrideHp = GetNullableInt(data, "overrideHitPoints");
            var baseHp = GetInt(data, "baseHitPoints");

            return overrideHp ?? (baseHp + GetConstitutionHitPoints(data, level));
        }

        private static int GetConstitutionHitPoints(JsonElement data, int level)
        {
            const int ConstitutionAbilityId = 3;

            var score = GetAbilityScore(data, ConstitutionAbilityId);
            var modifier = (int)Math.Floor((score - 10) / 2.0);

            return modifier * level;
        }

        private static int GetAbilityScore(JsonElement data, int abilityId)
        {
            var baseScore = GetStatValue(data, "stats", abilityId);
            var bonusScore = GetStatValue(data, "bonusStats", abilityId);
            var overrideScore = GetStatValue(data, "overrideStats", abilityId);

            return overrideScore != 0 ? overrideScore : baseScore + bonusScore;
        }

        private static int GetStatValue(JsonElement data, string property, int abilityId)
        {
            if (!data.TryGetProperty(property, out var array) || array.ValueKind != JsonValueKind.Array)
            {
                return 0;
            }

            foreach (var entry in array.EnumerateArray())
            {
                if (GetInt(entry, "id") == abilityId)
                {
                    return GetInt(entry, "value");
                }
            }

            return 0;
        }

        private static int GetLevel(JsonElement data)
        {
            var level = 0;

            if (data.TryGetProperty("classes", out var classes) && classes.ValueKind == JsonValueKind.Array)
            {
                foreach (var classEntry in classes.EnumerateArray())
                {
                    level += GetInt(classEntry, "level");
                }
            }

            if (level == 0)
            {
                level = GetInt(data, "level");
            }

            return level;
        }

        private static string? GetClassName(JsonElement data)
        {
            var names = new List<string>();

            if (data.TryGetProperty("classes", out var classes) && classes.ValueKind == JsonValueKind.Array)
            {
                foreach (var classEntry in classes.EnumerateArray())
                {
                    var name = GetString(GetObject(classEntry, "definition"), "name");
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        names.Add(name!);
                    }
                }
            }

            return names.Count > 0 ? string.Join(", ", names) : null;
        }

        private static JsonElement GetObject(JsonElement element, string property)
        {
            return element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.Object
                ? value
                : default;
        }

        private static string? GetString(JsonElement element, string property)
        {
            if (element.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            if (element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String)
            {
                return value.GetString();
            }

            return null;
        }

        private static long GetLong(JsonElement element, string property)
        {
            if (element.TryGetProperty(property, out var value))
            {
                if (value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out var result))
                {
                    return result;
                }

                if (value.ValueKind == JsonValueKind.String && long.TryParse(value.GetString(), out var parsed))
                {
                    return parsed;
                }
            }

            return 0;
        }

        private static int GetInt(JsonElement element, string property)
        {
            if (element.ValueKind == JsonValueKind.Undefined)
            {
                return 0;
            }

            return GetNullableInt(element, property) ?? 0;
        }

        private static int? GetNullableInt(JsonElement element, string property)
        {
            if (element.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            if (!element.TryGetProperty(property, out var value) || value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var result))
            {
                return result;
            }

            return null;
        }
    }
}
