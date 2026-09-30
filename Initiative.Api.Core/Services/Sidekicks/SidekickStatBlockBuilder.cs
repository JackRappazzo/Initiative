using Initiative.Persistence.Models.Parties;
using MongoDB.Bson;

namespace Initiative.Api.Core.Services.Sidekicks
{
    public interface ISidekickStatBlockBuilder
    {
        /// <summary>
        /// Merges a base creature's 5etools raw data with the sidekick's class features and
        /// leveling state to produce a renderable statblock snapshot.
        /// </summary>
        BsonDocument Build(string sidekickName, Sidekick sidekick, BsonDocument baseRawData);
    }

    public class SidekickStatBlockBuilder : ISidekickStatBlockBuilder
    {
        private const int BaseCreatureProficiencyBonus = 2; // CR 1/2 or lower creatures all have PB +2

        private static readonly string[] AbilityKeys = { "str", "dex", "con", "int", "wis", "cha" };

        private static readonly IReadOnlySet<string> SaveKeys = new HashSet<string>(AbilityKeys);

        private static readonly Dictionary<string, string> SkillAbilities = new()
        {
            ["acrobatics"] = "dex",
            ["animal handling"] = "wis",
            ["arcana"] = "int",
            ["athletics"] = "str",
            ["deception"] = "cha",
            ["history"] = "int",
            ["insight"] = "wis",
            ["intimidation"] = "cha",
            ["investigation"] = "int",
            ["medicine"] = "wis",
            ["nature"] = "int",
            ["perception"] = "wis",
            ["performance"] = "cha",
            ["persuasion"] = "cha",
            ["religion"] = "int",
            ["sleight of hand"] = "dex",
            ["stealth"] = "dex",
            ["survival"] = "wis",
        };

        private static readonly string[] WarriorBonusSaves = { "str", "con" };

        public BsonDocument Build(string sidekickName, Sidekick sidekick, BsonDocument baseRawData)
        {
            var doc = (BsonDocument)baseRawData.Clone();
            doc["name"] = sidekickName;

            var proficiencyBonus = SidekickRules.ProficiencyBonus(sidekick.Level);

            // ── Ability scores (with ASIs already applied) ──────────────────────
            var baseScores = ReadAbilityScores(baseRawData);
            doc["str"] = sidekick.AbilityScores.Strength;
            doc["dex"] = sidekick.AbilityScores.Dexterity;
            doc["con"] = sidekick.AbilityScores.Constitution;
            doc["int"] = sidekick.AbilityScores.Intelligence;
            doc["wis"] = sidekick.AbilityScores.Wisdom;
            doc["cha"] = sidekick.AbilityScores.Charisma;

            // ── Hit points ───────────────────────────────────────────────────────
            var conMod = sidekick.AbilityScores.ConstitutionModifier;
            doc["hp"] = new BsonDocument
            {
                { "average", sidekick.MaxHitPoints },
                { "formula", $"{sidekick.HitDiceCount}{sidekick.HitDieSize} + {conMod * sidekick.HitDiceCount}" }
            };
            doc["pb"] = proficiencyBonus;

            // ── Proficiencies & expertise derived from choices ──────────────────
            var skillProficiencyChoices = new HashSet<string>();
            var saveProficiencyChoices = new HashSet<string>();
            var expertiseSkills = new HashSet<string>();

            foreach (var choice in sidekick.Choices)
            {
                switch (choice.Type)
                {
                    case "Proficiencies":
                        foreach (var value in choice.Values)
                        {
                            if (SaveKeys.Contains(value)) saveProficiencyChoices.Add(value);
                            else if (SkillAbilities.ContainsKey(value)) skillProficiencyChoices.Add(value);
                        }
                        break;
                    case "Expertise":
                        expertiseSkills.UnionWith(choice.Values.Where(SkillAbilities.ContainsKey));
                        break;
                }
            }

            if (sidekick.Class == SidekickClass.Warrior)
            {
                saveProficiencyChoices.UnionWith(WarriorBonusSaves);
            }

            doc["save"] = BuildSaveDocument(baseRawData, baseScores, sidekick, proficiencyBonus, saveProficiencyChoices);
            var skillDoc = BuildSkillDocument(baseRawData, baseScores, sidekick, proficiencyBonus, skillProficiencyChoices, expertiseSkills);
            doc["skill"] = skillDoc;

            if (skillDoc.TryGetValue("perception", out var perception))
            {
                doc["passive"] = 10 + ParseModifier(perception);
            }

            // ── Sidekicks do not have a challenge rating ────────────────────────
            doc["cr"] = BsonNull.Value;

            // ── Class features appended as traits ────────────────────────────────
            doc["trait"] = BuildTraitArray(baseRawData, sidekick);

            // ── Spellcasting ─────────────────────────────────────────────────────
            if (sidekick.Spellcasting != null)
            {
                doc["spellcasting"] = new BsonArray { BuildSpellcasting(sidekick, proficiencyBonus) };
            }

            return doc;
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        private static SidekickAbilityScores ReadAbilityScores(BsonDocument raw)
            => new()
            {
                Strength = GetInt(raw, "str"),
                Dexterity = GetInt(raw, "dex"),
                Constitution = GetInt(raw, "con"),
                Intelligence = GetInt(raw, "int"),
                Wisdom = GetInt(raw, "wis"),
                Charisma = GetInt(raw, "cha"),
            };

        private static int GetInt(BsonDocument doc, string key)
            => doc.TryGetValue(key, out var value) && value.IsNumeric ? value.ToInt32() : 0;

        private static int ParseModifier(BsonValue value)
        {
            if (value.IsString)
            {
                return int.TryParse(value.AsString.TrimStart('+'), out var parsed) ? parsed : 0;
            }

            return value.IsNumeric ? value.ToInt32() : 0;
        }

        private static int BaseModifier(int score) => (int)Math.Floor((score - 10) / 2.0);

        private static int GetBaseScore(SidekickAbilityScores scores, string key) => key switch
        {
            "str" => scores.Strength,
            "dex" => scores.Dexterity,
            "con" => scores.Constitution,
            "int" => scores.Intelligence,
            "wis" => scores.Wisdom,
            "cha" => scores.Charisma,
            _ => 10
        };

        private static int GetNewScore(SidekickAbilityScores scores, string key) => key switch
        {
            "str" => scores.Strength,
            "dex" => scores.Dexterity,
            "con" => scores.Constitution,
            "int" => scores.Intelligence,
            "wis" => scores.Wisdom,
            "cha" => scores.Charisma,
            _ => 10
        };

        private static BsonDocument BuildSaveDocument(
            BsonDocument baseRaw,
            SidekickAbilityScores baseScores,
            Sidekick sidekick,
            int proficiencyBonus,
            HashSet<string> chosenSaves)
        {
            var result = new BsonDocument();
            var baseSaves = baseRaw.TryGetValue("save", out var saveVal) && saveVal is BsonDocument saveDoc
                ? saveDoc
                : null;

            foreach (var key in AbilityKeys)
            {
                var isProficient = chosenSaves.Contains(key);
                if (baseSaves != null && baseSaves.TryGetValue(key, out var baseValue))
                {
                    var baseModifier = BaseModifier(GetBaseScore(baseScores, key));
                    if (ParseModifier(baseValue) - baseModifier >= BaseCreatureProficiencyBonus)
                    {
                        isProficient = true;
                    }
                }

                if (!isProficient && baseSaves != null && !baseSaves.Contains(key))
                {
                    continue;
                }

                var newModifier = BaseModifier(GetNewScore(sidekick.AbilityScores, key));
                result[key] = FormatModifier(newModifier + (isProficient ? proficiencyBonus : 0));
            }

            return result;
        }

        private static BsonDocument BuildSkillDocument(
            BsonDocument baseRaw,
            SidekickAbilityScores baseScores,
            Sidekick sidekick,
            int proficiencyBonus,
            HashSet<string> chosenSkills,
            HashSet<string> expertiseSkills)
        {
            var result = new BsonDocument();
            var baseSkills = baseRaw.TryGetValue("skill", out var skillVal) && skillVal is BsonDocument skillDoc
                ? skillDoc
                : null;

            foreach (var skillKey in SkillAbilities.Keys)
            {
                var abilityKey = SkillAbilities[skillKey];
                int multiplier = 0;

                if (baseSkills != null && baseSkills.TryGetValue(skillKey, out var baseValue))
                {
                    var baseModifier = BaseModifier(GetBaseScore(baseScores, abilityKey));
                    var diff = ParseModifier(baseValue) - baseModifier;
                    multiplier = diff >= 2 * BaseCreatureProficiencyBonus ? 2
                        : diff >= BaseCreatureProficiencyBonus ? 1
                        : 0;
                }

                if (expertiseSkills.Contains(skillKey)) multiplier = Math.Max(multiplier, 2);
                if (chosenSkills.Contains(skillKey)) multiplier = Math.Max(multiplier, 1);

                if (multiplier == 0)
                {
                    continue;
                }

                var newModifier = BaseModifier(GetNewScore(sidekick.AbilityScores, abilityKey));
                result[skillKey] = FormatModifier(newModifier + proficiencyBonus * multiplier);
            }

            return result;
        }

        private static string FormatModifier(int value) => value >= 0 ? $"+{value}" : value.ToString();

        private static BsonArray BuildTraitArray(BsonDocument baseRaw, Sidekick sidekick)
        {
            var traits = new BsonArray();

            if (baseRaw.TryGetValue("trait", out var baseTrait) && baseTrait is BsonArray baseTraits)
            {
                traits.AddRange(baseTraits);
            }

            foreach (var feature in SidekickRules.FeaturesFor(sidekick.Class, sidekick.Level))
            {
                traits.Add(new BsonDocument
                {
                    { "name", feature.Name },
                    { "entries", new BsonArray { feature.Description } }
                });
            }

            return traits;
        }

        private static BsonDocument BuildSpellcasting(Sidekick sidekick, int proficiencyBonus)
        {
            var spellcasting = sidekick.Spellcasting!;
            var abilityModifier = BaseModifier(GetNewScore(sidekick.AbilityScores, spellcasting.Ability));
            var saveDc = 8 + proficiencyBonus + abilityModifier;
            var attackBonus = proficiencyBonus + abilityModifier;

            var doc = new BsonDocument
            {
                { "name", "Spellcasting" },
                { "type", "spellcasting" },
                { "ability", spellcasting.Ability },
                { "headerEntries", new BsonArray { $"{spellcasting.Kind} sidekick spellcaster (spell save DC {saveDc}, +{attackBonus} to hit with spell attacks)." } }
            };

            if (spellcasting.Cantrips.Count > 0)
            {
                doc["will"] = new BsonArray(spellcasting.Cantrips.Select(c => (BsonValue)c));
            }

            var spells = new BsonDocument();
            foreach (var kv in spellcasting.KnownSpells.Where(kv => kv.Value.Count > 0).OrderBy(kv => kv.Key))
            {
                spells[kv.Key.ToString()] = new BsonDocument
                {
                    { "slots", spellcasting.Slots.TryGetValue(kv.Key, out var slots) ? slots : 0 },
                    { "spells", new BsonArray(kv.Value.Select(s => (BsonValue)s)) }
                };
            }

            if (spells.ElementCount > 0)
            {
                doc["spells"] = spells;
            }

            return doc;
        }
    }
}
