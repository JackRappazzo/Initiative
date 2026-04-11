using MongoDB.Bson;

namespace Initiative.Api.Core.Services.Bestiary
{
    public interface ICustomCreatureRawDataBuilder
    {
        BsonDocument Build(CustomCreatureData data);
    }

    public class CustomCreatureRawDataBuilder : ICustomCreatureRawDataBuilder
    {
        public BsonDocument Build(CustomCreatureData data)
        {
            var doc = new BsonDocument { { "name", data.Name } };

            // Identity
            if (data.Size != null)
                doc["size"] = new BsonArray { data.Size };

            if (data.CreatureType != null || data.Subtype != null)
            {
                if (data.Subtype != null)
                    doc["type"] = new BsonDocument { { "type", data.CreatureType ?? string.Empty }, { "tags", new BsonArray { data.Subtype } } };
                else
                    doc["type"] = data.CreatureType != null ? (BsonValue)data.CreatureType : BsonNull.Value;
            }

            if (data.Alignment != null)
                doc["alignment"] = new BsonArray { data.Alignment };

            if (data.ChallengeRating != null)
                doc["cr"] = data.ChallengeRating;

            if (data.ProficiencyBonus.HasValue)
                doc["pb"] = data.ProficiencyBonus.Value;

            // HP
            if (data.HP.HasValue || data.HitDice != null)
            {
                var hpDoc = new BsonDocument();
                if (data.HP.HasValue) hpDoc["average"] = data.HP.Value;
                if (data.HitDice != null) hpDoc["formula"] = data.HitDice;
                doc["hp"] = hpDoc;
            }

            // AC
            if (data.AC.HasValue)
            {
                if (data.AcNote != null)
                    doc["ac"] = new BsonArray { new BsonDocument { { "ac", data.AC.Value }, { "from", new BsonArray { data.AcNote } } } };
                else
                    doc["ac"] = new BsonArray { data.AC.Value };
            }

            // Ability scores
            var ab = data.AbilityScores;
            if (ab != null)
            {
                if (ab.Str.HasValue) doc["str"] = ab.Str.Value;
                if (ab.Dex.HasValue) doc["dex"] = ab.Dex.Value;
                if (ab.Con.HasValue) doc["con"] = ab.Con.Value;
                if (ab.Int.HasValue) doc["int"] = ab.Int.Value;
                if (ab.Wis.HasValue) doc["wis"] = ab.Wis.Value;
                if (ab.Cha.HasValue) doc["cha"] = ab.Cha.Value;
            }

            // Speed
            var spd = data.Speed;
            if (spd != null)
            {
                var spdDoc = new BsonDocument();
                if (spd.Walk.HasValue) spdDoc["walk"] = spd.Walk.Value;
                if (spd.Fly.HasValue)
                {
                    if (spd.CanHover == true)
                        spdDoc["fly"] = new BsonDocument { { "number", spd.Fly.Value }, { "condition", "(hover)" } };
                    else
                        spdDoc["fly"] = spd.Fly.Value;
                }
                if (spd.Swim.HasValue) spdDoc["swim"] = spd.Swim.Value;
                if (spd.Burrow.HasValue) spdDoc["burrow"] = spd.Burrow.Value;
                if (spd.Climb.HasValue) spdDoc["climb"] = spd.Climb.Value;
                if (spdDoc.ElementCount > 0) doc["speed"] = spdDoc;
            }

            // Saving throws
            if (data.SavingThrows?.Count > 0)
            {
                var saveDoc = new BsonDocument();
                foreach (var kv in data.SavingThrows) saveDoc[kv.Key] = kv.Value;
                doc["save"] = saveDoc;
            }

            // Skills
            if (data.Skills?.Count > 0)
            {
                var skillDoc = new BsonDocument();
                foreach (var kv in data.Skills) skillDoc[kv.Key] = kv.Value;
                doc["skill"] = skillDoc;
            }

            // Damage / condition modifiers
            if (data.DamageResistances?.Count > 0)
                doc["resist"] = new BsonArray(data.DamageResistances.Select(r => (BsonValue)r));

            if (data.DamageImmunities?.Count > 0)
                doc["immune"] = new BsonArray(data.DamageImmunities.Select(r => (BsonValue)r));

            if (data.DamageVulnerabilities?.Count > 0)
                doc["vulnerable"] = new BsonArray(data.DamageVulnerabilities.Select(r => (BsonValue)r));

            if (data.ConditionImmunities?.Count > 0)
                doc["conditionImmune"] = new BsonArray(data.ConditionImmunities.Select(r => (BsonValue)r));

            // Senses / languages
            if (data.Senses?.Count > 0)
                doc["senses"] = new BsonArray(data.Senses.Select(s => (BsonValue)s));

            if (data.Languages?.Count > 0)
                doc["languages"] = new BsonArray(data.Languages.Select(l => (BsonValue)l));

            // Traits (free text, stored in a single entry)
            if (!string.IsNullOrWhiteSpace(data.Traits))
                doc["trait"] = new BsonArray
                {
                    new BsonDocument { { "name", "Traits" }, { "entries", new BsonArray { data.Traits } } }
                };

            // Action sections
            static BsonArray EntriesToBson(List<CustomCreatureEntry> entries) =>
                new(entries.Select(e => (BsonValue)new BsonDocument
                {
                    { "name", e.Name },
                    { "entries", new BsonArray { e.Description } }
                }));

            if (data.Actions?.Count > 0) doc["action"] = EntriesToBson(data.Actions);
            if (data.BonusActions?.Count > 0) doc["bonus"] = EntriesToBson(data.BonusActions);
            if (data.Reactions?.Count > 0) doc["reaction"] = EntriesToBson(data.Reactions);
            if (data.LegendaryActions?.Count > 0) doc["legendary"] = EntriesToBson(data.LegendaryActions);
            if (data.LegendaryActionCount.HasValue) doc["legendaryActionsLair"] = data.LegendaryActionCount.Value;

            // Spellcasting
            var sc = data.Spellcasting;
            var hasSpellcasting = sc != null && (
                sc.SlotSpells?.Count > 0 ||
                sc.DailySpells?.Count > 0 ||
                !string.IsNullOrWhiteSpace(sc.Description) ||
                !string.IsNullOrWhiteSpace(sc.FreeformText));
            if (hasSpellcasting)
            {
                var scDoc = new BsonDocument { { "name", "Spellcasting" }, { "type", "spellcasting" } };
                if (sc!.Ability != null) scDoc["ability"] = sc.Ability;

                // Build headerEntries: [description?, dc/bonus line?]
                var headerEntries = new BsonArray();
                if (!string.IsNullOrWhiteSpace(sc.Description))
                    headerEntries.Add(sc.Description.Trim());
                var statParts = new List<string>();
                if (sc.SpellSaveDc.HasValue) statParts.Add($"spell save DC {sc.SpellSaveDc.Value}");
                if (sc.SpellAttackBonus.HasValue) statParts.Add($"+{sc.SpellAttackBonus.Value} to spell attack rolls");
                if (statParts.Count > 0)
                    headerEntries.Add(string.Join(", ", statParts));
                if (headerEntries.Count > 0) scDoc["headerEntries"] = headerEntries;

                if (!string.IsNullOrWhiteSpace(sc.FreeformText))
                {
                    // Freeform: store each non-empty line as a separate headerEntry after description/stats
                    var lines = sc.FreeformText.Split('\n')
                        .Select(l => l.TrimEnd())
                        .Where(l => l.Length > 0);
                    foreach (var line in lines)
                        headerEntries.Add(line);
                }
                else
                {
                    // Slot-based: level 0 = at-will/cantrips
                    if (sc.SlotSpells?.Count > 0)
                    {
                        var spellsDoc = new BsonDocument();
                        foreach (var kv in sc.SlotSpells)
                        {
                            var spellArray = new BsonArray(kv.Value.Select(s => (BsonValue)s));
                            if (kv.Key == "0")
                                scDoc["will"] = spellArray;
                            else
                                spellsDoc[kv.Key] = new BsonDocument { { "slots", int.TryParse(kv.Key, out var lvl) ? lvl : 1 }, { "spells", spellArray } };
                        }
                        if (spellsDoc.ElementCount > 0) scDoc["spells"] = spellsDoc;
                    }

                    // X/day
                    if (sc.DailySpells?.Count > 0)
                    {
                        var dailyDoc = new BsonDocument();
                        foreach (var entry in sc.DailySpells)
                        {
                            // Format: "3/day: Fireball" or "1/day each: Misty Step, Fly"
                            var match = System.Text.RegularExpressions.Regex.Match(entry, @"^(\d+)/day(?:\s+each)?:\s*(.+)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (match.Success)
                            {
                                var count = match.Groups[1].Value;
                                var spells = match.Groups[2].Value.Split(',').Select(s => (BsonValue)s.Trim()).ToList();
                                var key = count + "e"; // 5etools daily key convention: "3e" = 3/day each
                                dailyDoc[key] = new BsonArray(spells);
                            }
                        }
                        if (dailyDoc.ElementCount > 0) scDoc["daily"] = dailyDoc;
                    }
                }

                doc["spellcasting"] = new BsonArray { scDoc };
            }

            return doc;
        }
    }
}
