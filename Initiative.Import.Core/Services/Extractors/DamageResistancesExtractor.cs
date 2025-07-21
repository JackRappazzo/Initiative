using System.Text.Json;
using Initiative.Import.Core.Models;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Service to extract damage resistances from JSON data
    /// </summary>
    public class DamageResistancesExtractor : IDamageResistancesExtractor
    {
        /// <summary>
        /// Extracts damage resistances from JSON elements which can be strings or objects with special properties
        /// </summary>
        /// <param name="damageResistancesList">List of JSON elements representing damage resistances</param>
        /// <returns>List of damage resistance objects</returns>
        public List<DamageResistanceJson> ExtractDamageResistances(List<JsonElement>? damageResistancesList)
        {
            if (damageResistancesList == null || !damageResistancesList.Any())
                return new List<DamageResistanceJson>();

            var resistances = new List<DamageResistanceJson>();

            foreach (var element in damageResistancesList)
            {
                if (element.ValueKind == JsonValueKind.String)
                {
                    // Simple string damage type
                    var damageTypeString = element.GetString() ?? string.Empty;
                    var resistance = new DamageResistanceJson
                    {
                        RawValue = damageTypeString,
                        DamageType = ParseDamageType(damageTypeString)
                    };
                    resistances.Add(resistance);
                }
                else if (element.ValueKind == JsonValueKind.Object)
                {
                    // Object with special property
                    if (element.TryGetProperty("special", out var specialProperty) &&
                        specialProperty.ValueKind == JsonValueKind.String)
                    {
                        var specialValue = specialProperty.GetString() ?? string.Empty;
                        var resistance = new DamageResistanceJson
                        {
                            Special = specialValue,
                            DamageType = DamageType.Special,
                            RawValue = specialValue
                        };
                        resistances.Add(resistance);
                    }
                }
            }

            return resistances;
        }

        /// <summary>
        /// Parses a string into a DamageType enum value
        /// </summary>
        /// <param name="damageTypeString">The damage type string</param>
        /// <returns>Corresponding DamageType enum value or null if not recognized</returns>
        private DamageType? ParseDamageType(string damageTypeString)
        {
            if (string.IsNullOrEmpty(damageTypeString))
                return null;

            // Handle common string variations and map them to enum values
            var normalizedString = damageTypeString.ToLowerInvariant().Trim();
            
            return normalizedString switch
            {
                "acid" => DamageType.Acid,
                "bludgeoning" => DamageType.Bludgeoning,
                "cold" => DamageType.Cold,
                "fire" => DamageType.Fire,
                "force" => DamageType.Force,
                "lightning" => DamageType.Lightning,
                "necrotic" => DamageType.Necrotic,
                "piercing" => DamageType.Piercing,
                "poison" => DamageType.Poison,
                "psychic" => DamageType.Psychic,
                "radiant" => DamageType.Radiant,
                "slashing" => DamageType.Slashing,
                "thunder" => DamageType.Thunder,
                _ => null // Return null for unrecognized damage types
            };
        }
    }
}