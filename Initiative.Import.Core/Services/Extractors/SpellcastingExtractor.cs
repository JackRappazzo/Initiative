using Initiative.Import.Core.Models;
using System.Text.RegularExpressions;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Service to extract spellcasting information from JSON data
    /// </summary>
    public class SpellcastingExtractor : ISpellcastingExtractor
    {
        /// <summary>
        /// Extracts spellcasting information from a list of SpellcastingJson objects
        /// </summary>
        /// <param name="spellcastingList">List of spellcasting JSON objects</param>
        /// <param name="charismaModifier">The creature's charisma modifier (used for default calculations)</param>
        /// <param name="proficiencyBonus">The creature's proficiency bonus</param>
        /// <returns>Tuple containing spellcasting details</returns>
        public (bool IsSpellcaster, string? SpellcastingAbility, int? SpellSaveDC, int? SpellAttackBonus, Dictionary<int, int> SpellSlots, Dictionary<int, List<string>> SpellsKnown) ExtractSpellcasting(List<SpellcastingJson>? spellcastingList, int charismaModifier, int proficiencyBonus)
        {
            if (spellcastingList == null || !spellcastingList.Any())
            {
                return (false, null, null, null, new Dictionary<int, int>(), new Dictionary<int, List<string>>());
            }

            var spellSlots = new Dictionary<int, int>();
            var spellsKnown = new Dictionary<int, List<string>>();
            string? spellcastingAbility = null;
            int? spellSaveDC = null;
            int? spellAttackBonus = null;

            foreach (var spellcasting in spellcastingList)
            {
                // Extract spellcasting ability
                if (!string.IsNullOrEmpty(spellcasting.SpellcastingAbility))
                {
                    spellcastingAbility = spellcasting.SpellcastingAbility;
                }

                // Extract spell save DC from header entries
                if (spellcasting.HeaderEntries != null)
                {
                    foreach (var entry in spellcasting.HeaderEntries)
                    {
                        // Try to match both {@dc 17} format and "spell save DC 17" format
                        var dcMatch = Regex.Match(entry, @"spell save {@dc (\d+)}|spell save DC (\d+)", RegexOptions.IgnoreCase);
                        if (dcMatch.Success)
                        {
                            var dcValue = dcMatch.Groups[1].Success ? dcMatch.Groups[1].Value : dcMatch.Groups[2].Value;
                            if (int.TryParse(dcValue, out var dc))
                            {
                                spellSaveDC = dc;
                            }
                        }

                        var attackMatch = Regex.Match(entry, @"spell attack (\+?\d+)", RegexOptions.IgnoreCase);
                        if (attackMatch.Success && int.TryParse(attackMatch.Groups[1].Value, out var attack))
                        {
                            spellAttackBonus = attack;
                        }
                    }
                }

                // Extract at-will spells (cantrips/level 0)
                if (spellcasting.AtWill != null && spellcasting.AtWill.Any())
                {
                    var atWillSpells = ExtractSpellNames(spellcasting.AtWill);
                    if (atWillSpells.Any())
                    {
                        spellsKnown[0] = atWillSpells;
                    }
                }

                // Extract daily spells
                if (spellcasting.Daily != null)
                {
                    foreach (var dailyEntry in spellcasting.Daily)
                    {
                        var (level, slots) = ParseDailyKey(dailyEntry.Key);
                        if (level > 0)
                        {
                            if (slots > 0)
                            {
                                spellSlots[level] = slots;
                            }

                            var dailySpells = ExtractSpellNames(dailyEntry.Value);
                            if (dailySpells.Any())
                            {
                                if (spellsKnown.ContainsKey(level))
                                {
                                    spellsKnown[level].AddRange(dailySpells);
                                }
                                else
                                {
                                    spellsKnown[level] = dailySpells;
                                }
                            }
                        }
                    }
                }
            }

            // Calculate defaults if not found
            if (spellSaveDC == null && !string.IsNullOrEmpty(spellcastingAbility))
            {
                var abilityModifier = GetAbilityModifier(spellcastingAbility, charismaModifier);
                spellSaveDC = 8 + proficiencyBonus + abilityModifier;
            }

            if (spellAttackBonus == null && !string.IsNullOrEmpty(spellcastingAbility))
            {
                var abilityModifier = GetAbilityModifier(spellcastingAbility, charismaModifier);
                spellAttackBonus = proficiencyBonus + abilityModifier;
            }

            bool isSpellcaster = spellsKnown.Any() || spellSlots.Any();

            return (isSpellcaster, spellcastingAbility, spellSaveDC, spellAttackBonus, spellSlots, spellsKnown);
        }

        /// <summary>
        /// Extracts clean spell names from spell references
        /// </summary>
        /// <param name="spellList">List of spell references</param>
        /// <returns>List of clean spell names</returns>
        private List<string> ExtractSpellNames(List<string> spellList)
        {
            var spellNames = new List<string>();

            foreach (var spell in spellList)
            {
                // Extract spell name from {@spell SpellName|Source} format
                var match = Regex.Match(spell, @"{@spell ([^|}]+)");
                if (match.Success)
                {
                    spellNames.Add(match.Groups[1].Value.Trim());
                }
                else
                {
                    // If not in the expected format, use the string as-is (cleaned up)
                    var cleanSpell = spell.Replace("{@spell ", "").Replace("}", "").Split('|')[0].Trim();
                    if (!string.IsNullOrEmpty(cleanSpell))
                    {
                        spellNames.Add(cleanSpell);
                    }
                }
            }

            return spellNames;
        }

        /// <summary>
        /// Parses daily spell keys to extract level and slots
        /// </summary>
        /// <param name="dailyKey">Daily key like "1e", "3", etc.</param>
        /// <returns>Tuple of (level, slots)</returns>
        private (int level, int slots) ParseDailyKey(string dailyKey)
        {
            if (string.IsNullOrEmpty(dailyKey))
                return (0, 0);

            // Handle formats like "1e" (1 slot each), "3" (3 slots total), etc.
            var match = Regex.Match(dailyKey, @"(\d+)([e]?)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var number))
            {
                var isEach = match.Groups[2].Value == "e";
                
                if (isEach)
                {
                    // "1e" means level 1, 1 slot each (so 1 slot total)
                    // "3e" means level 3, 1 slot each (so 1 slot total)
                    return (number, 1);
                }
                else
                {
                    // "3" means level 3, 3 total slots
                    // "5" means level 5, 5 total slots
                    return (number, number);
                }
            }

            return (0, 0);
        }

        /// <summary>
        /// Gets the ability modifier for the given ability name
        /// </summary>
        /// <param name="abilityName">Name of the ability (cha, int, wis, etc.)</param>
        /// <param name="charismaModifier">Fallback charisma modifier</param>
        /// <returns>Ability modifier</returns>
        private int GetAbilityModifier(string abilityName, int charismaModifier)
        {
            // For now, default to charisma modifier
            // In a more complete implementation, you'd pass all ability scores
            return abilityName.ToLowerInvariant() switch
            {
                "cha" or "charisma" => charismaModifier,
                _ => charismaModifier // Default to charisma for dragons and other common spellcasters
            };
        }
    }
}