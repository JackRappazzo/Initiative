using System.Text.Json;

namespace Initiative.Api.Core.Services.DndBeyond
{
    public class DndBeyondProxyService : IDndBeyondProxyService
    {
        private const int StrengthAbilityId = 1;
        private const int DexterityAbilityId = 2;
        private const int ConstitutionAbilityId = 3;
        private const int IntelligenceAbilityId = 4;
        private const int WisdomAbilityId = 5;
        private const int CharismaAbilityId = 6;

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
                TemporaryHP = GetInt(data, "temporaryHitPoints"),

                Strength = GetAbilityScore(data, StrengthAbilityId),
                Dexterity = GetAbilityScore(data, DexterityAbilityId),
                Constitution = GetAbilityScore(data, ConstitutionAbilityId),
                Intelligence = GetAbilityScore(data, IntelligenceAbilityId),
                Wisdom = GetAbilityScore(data, WisdomAbilityId),
                Charisma = GetAbilityScore(data, CharismaAbilityId),

                ArmorClass = CalculateArmorClass(data),
                ProficiencyBonus = 1 + (int)Math.Ceiling(level / 4.0),
                Race = GetRaceName(data),
                Speed = CalculateSpeed(data),

                SpellSlots = ParseSpellSlots(data, "spellSlots"),
                PactSlots = ParseSpellSlots(data, "pactMagic"),
                PreparedSpells = ParsePreparedSpells(data),
                FocusPoints = ParseFocusPoints(data),
                Attacks = ParseAttacks(data)
            };
        }

        internal static int CalculateMaxHitPoints(JsonElement data, int level)
        {
            var overrideHp = GetNullableInt(data, "overrideHitPoints");
            var baseHp = GetInt(data, "baseHitPoints");

            return overrideHp ?? (baseHp + GetConstitutionHitPoints(data, level));
        }

        internal static int CalculateArmorClass(JsonElement data)
        {
            var dexModifier = AbilityModifier(GetAbilityScore(data, DexterityAbilityId));
            var modifiers = GetModifiers(data).ToList();

            // Unarmored Defense (e.g. Monk: 10 + DEX + WIS; Barbarian: 10 + DEX + CON)
            foreach (var modifier in modifiers)
            {
                if (GetString(modifier, "type") != "set" || GetString(modifier, "subType") != "unarmored-armor-class")
                {
                    continue;
                }

                var secondaryScore = GetAbilityScore(data, GetInt(modifier, "statId"));
                return 10 + dexModifier + AbilityModifier(secondaryScore) + SumArmorClassBonuses(modifiers);
            }

            int baseArmor = 10;
            int dexCap = int.MaxValue;
            int shieldBonus = 0;
            bool hasArmor = false;

            if (data.TryGetProperty("inventory", out var inventory) && inventory.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in inventory.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object || !GetBool(item, "equipped"))
                    {
                        continue;
                    }

                    var definition = GetObject(item, "definition");
                    var armorClass = GetNullableInt(definition, "armorClass");
                    if (armorClass is null)
                    {
                        continue;
                    }

                    var type = GetString(definition, "type") ?? "";
                    if (type.Contains("Shield", StringComparison.OrdinalIgnoreCase))
                    {
                        shieldBonus += armorClass.Value;
                    }
                    else
                    {
                        baseArmor = armorClass.Value;
                        hasArmor = true;

                        if (type.Contains("Heavy", StringComparison.OrdinalIgnoreCase))
                        {
                            dexCap = 0;
                        }
                        else if (type.Contains("Medium", StringComparison.OrdinalIgnoreCase))
                        {
                            dexCap = 2;
                        }
                    }
                }
            }

            var dexContribution = hasArmor ? Math.Min(dexModifier, dexCap) : dexModifier;
            return baseArmor + dexContribution + shieldBonus + SumArmorClassBonuses(modifiers);
        }

        internal static int CalculateSpeed(JsonElement data)
        {
            int baseSpeed = 0;
            if (data.TryGetProperty("race", out var race) && race.ValueKind == JsonValueKind.Object)
            {
                var normal = GetObject(GetObject(race, "weightSpeeds"), "normal");
                baseSpeed = GetInt(normal, "walk");
            }

            int bonus = 0;
            foreach (var modifier in GetModifiers(data))
            {
                var subType = GetString(modifier, "subType") ?? "";
                if (GetString(modifier, "type") == "bonus" && GetBool(modifier, "isGranted")
                    && subType.Contains("movement", StringComparison.OrdinalIgnoreCase))
                {
                    bonus += GetInt(modifier, "value");
                }
            }

            return baseSpeed + bonus;
        }

        internal static List<DndBeyondSpellSlot> ParseSpellSlots(JsonElement data, string property)
        {
            var result = new List<DndBeyondSpellSlot>();

            if (!data.TryGetProperty(property, out var array) || array.ValueKind != JsonValueKind.Array)
            {
                return result;
            }

            foreach (var entry in array.EnumerateArray())
            {
                if (entry.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                var slot = new DndBeyondSpellSlot
                {
                    Level = GetInt(entry, "level"),
                    Used = GetInt(entry, "used"),
                    Available = GetInt(entry, "available")
                };

                if (slot.Level > 0 && slot.Used + slot.Available > 0)
                {
                    result.Add(slot);
                }
            }

            return result;
        }

        internal static List<DndBeyondPreparedSpell> ParsePreparedSpells(JsonElement data)
        {
            var result = new List<DndBeyondPreparedSpell>();
            var seen = new HashSet<string>();

            if (data.TryGetProperty("classSpells", out var classSpells) && classSpells.ValueKind == JsonValueKind.Array)
            {
                foreach (var classEntry in classSpells.EnumerateArray())
                {
                    if (classEntry.ValueKind != JsonValueKind.Object
                        || !classEntry.TryGetProperty("spells", out var spells)
                        || spells.ValueKind != JsonValueKind.Array)
                    {
                        continue;
                    }

                    foreach (var spell in spells.EnumerateArray())
                    {
                        AddPreparedSpell(spell, result, seen);
                    }
                }
            }

            if (data.TryGetProperty("spells", out var spellsRoot) && spellsRoot.ValueKind == JsonValueKind.Object)
            {
                foreach (var category in spellsRoot.EnumerateObject())
                {
                    if (category.Value.ValueKind != JsonValueKind.Array)
                    {
                        continue;
                    }

                    foreach (var spell in category.Value.EnumerateArray())
                    {
                        AddPreparedSpell(spell, result, seen);
                    }
                }
            }

            return result
                .OrderBy(spell => spell.Level)
                .ThenBy(spell => spell.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        internal static DndBeyondResource? ParseFocusPoints(JsonElement data)
        {
            if (!data.TryGetProperty("actions", out var actions) || actions.ValueKind != JsonValueKind.Object
                || !actions.TryGetProperty("class", out var classActions) || classActions.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            foreach (var action in classActions.EnumerateArray())
            {
                if (action.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                var name = GetString(action, "name");
                if (name is null || !name.Contains("Focus Point", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var limitedUse = GetObject(action, "limitedUse");
                var max = GetInt(limitedUse, "maxUses");
                if (max <= 0)
                {
                    continue;
                }

                var used = GetInt(limitedUse, "numberUsed");
                return new DndBeyondResource
                {
                    Current = Math.Max(max - used, 0),
                    Max = max
                };
            }

            return null;
        }

        internal static List<DndBeyondAttack> ParseAttacks(JsonElement data)
        {
            var result = new List<DndBeyondAttack>();

            if (!data.TryGetProperty("inventory", out var inventory) || inventory.ValueKind != JsonValueKind.Array)
            {
                return result;
            }

            var strength = GetAbilityScore(data, StrengthAbilityId);
            var dexterity = GetAbilityScore(data, DexterityAbilityId);
            var level = GetLevel(data);
            var proficiencyBonus = 1 + (int)Math.Ceiling(level / 4.0);
            var modifiers = GetModifiers(data).ToList();

            foreach (var item in inventory.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object || !GetBool(item, "equipped"))
                {
                    continue;
                }

                var definition = GetObject(item, "definition");
                var diceString = GetString(GetObject(definition, "damage"), "diceString");
                if (string.IsNullOrWhiteSpace(diceString))
                {
                    continue;
                }

                var name = GetString(definition, "name");
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var isRanged = GetInt(definition, "attackType") == 2;
                var hasFinesse = HasWeaponProperty(definition, "Finesse");
                var abilityModifier = isRanged
                    ? AbilityModifier(dexterity)
                    : hasFinesse
                        ? Math.Max(AbilityModifier(strength), AbilityModifier(dexterity))
                        : AbilityModifier(strength);

                var magicBonus = SumWeaponMagicBonus(definition);
                var proficient = IsProficientWithWeapon(modifiers, GetNullableInt(definition, "baseItemId"), GetInt(definition, "categoryId"));

                result.Add(new DndBeyondAttack
                {
                    Name = name!,
                    ToHitBonus = abilityModifier + (proficient ? proficiencyBonus : 0) + magicBonus,
                    DamageDice = diceString,
                    DamageBonus = abilityModifier + magicBonus,
                    DamageType = GetString(definition, "damageType"),
                    Range = GetInt(definition, "range"),
                    LongRange = GetInt(definition, "longRange"),
                    IsRanged = isRanged
                });
            }

            return result;
        }

        private static bool HasWeaponProperty(JsonElement definition, string propertyName)
        {
            if (!definition.TryGetProperty("properties", out var properties) || properties.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            foreach (var property in properties.EnumerateArray())
            {
                if (string.Equals(GetString(property, "name"), propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static int SumWeaponMagicBonus(JsonElement definition)
        {
            if (!definition.TryGetProperty("grantedModifiers", out var granted) || granted.ValueKind != JsonValueKind.Array)
            {
                return 0;
            }

            var total = 0;
            foreach (var modifier in granted.EnumerateArray())
            {
                if (GetString(modifier, "type") == "bonus" && GetString(modifier, "subType") == "magic")
                {
                    total += GetInt(modifier, "value");
                }
            }

            return total;
        }

        private static bool IsProficientWithWeapon(List<JsonElement> modifiers, int? baseItemId, int categoryId)
        {
            foreach (var modifier in modifiers)
            {
                if (GetString(modifier, "type") != "proficiency" || !GetBool(modifier, "isGranted"))
                {
                    continue;
                }

                if (baseItemId.HasValue && GetNullableInt(modifier, "entityId") == baseItemId.Value)
                {
                    return true;
                }

                var subType = GetString(modifier, "subType");
                if (subType == "simple-weapons" && categoryId == 1)
                {
                    return true;
                }

                if (subType == "martial-weapons" && categoryId == 2)
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddPreparedSpell(JsonElement spell, List<DndBeyondPreparedSpell> result, HashSet<string> seen)
        {
            if (spell.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            if (!GetBool(spell, "countsAsKnownSpell") && !GetBool(spell, "alwaysPrepared") && !GetBool(spell, "prepared"))
            {
                return;
            }

            var definition = GetObject(spell, "definition");
            var name = GetString(definition, "name");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var level = GetInt(definition, "level");
            var key = $"{level}|{name}";
            if (!seen.Add(key))
            {
                return;
            }

            result.Add(new DndBeyondPreparedSpell
            {
                Name = name!,
                Level = level,
                IsCantrip = level == 0
            });
        }

        private static int SumArmorClassBonuses(IEnumerable<JsonElement> modifiers)
        {
            var total = 0;

            foreach (var modifier in modifiers)
            {
                if (GetString(modifier, "type") == "bonus" && GetString(modifier, "subType") == "armor-class"
                    && GetBool(modifier, "isGranted"))
                {
                    total += GetInt(modifier, "value");
                }
            }

            return total;
        }

        private static IEnumerable<JsonElement> GetModifiers(JsonElement data)
        {
            if (!data.TryGetProperty("modifiers", out var modifiers) || modifiers.ValueKind != JsonValueKind.Object)
            {
                yield break;
            }

            foreach (var category in modifiers.EnumerateObject())
            {
                if (category.Value.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var entry in category.Value.EnumerateArray())
                {
                    yield return entry;
                }
            }
        }

        private static string? GetRaceName(JsonElement data)
        {
            var race = GetObject(data, "race");
            return GetString(race, "fullName") ?? GetString(race, "baseRaceName");
        }

        private static int GetConstitutionHitPoints(JsonElement data, int level)
        {
            var score = GetAbilityScore(data, ConstitutionAbilityId);
            var modifier = AbilityModifier(score);

            return modifier * level;
        }

        private static int AbilityModifier(int score)
        {
            return (int)Math.Floor((score - 10) / 2.0);
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

        private static bool GetBool(JsonElement element, string property)
        {
            if (element.ValueKind == JsonValueKind.Undefined)
            {
                return false;
            }

            if (element.TryGetProperty(property, out var value)
                && (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False))
            {
                return value.GetBoolean();
            }

            return false;
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
