using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Extension methods for converting between Import and Persistence models
    /// </summary>
    public static class ModelConversionExtensions
    {
        /// <summary>
        /// Converts a list of DamageResistanceJson to List of Persistence DamageType
        /// </summary>
        /// <param name="resistances">List of damage resistance JSON objects</param>
        /// <returns>List of persistence damage types</returns>
        public static List<DamageType> ToPersistenceDamageTypes(this List<DamageResistanceJson> resistances)
        {
            return resistances
                .Where(r => r.DamageType.HasValue)
                .Select(r => r.DamageType!.Value)
                .ToList();
        }

        /// <summary>
        /// Converts condition immunity strings to Persistence Condition enum values
        /// </summary>
        /// <param name="conditionNames">List of condition names</param>
        /// <returns>List of persistence condition enum values</returns>
        public static List<Condition> ToPersistenceConditions(this List<string> conditionNames)
        {
            return conditionNames
                .Select(ParseCondition)
                .Where(c => c.HasValue)
                .Select(c => c!.Value)
                .ToList();
        }

        /// <summary>
        /// Parses a condition name string to a Condition enum value
        /// </summary>
        /// <param name="conditionName">Condition name string</param>
        /// <returns>Condition enum value or null if not recognized</returns>
        private static Condition? ParseCondition(string conditionName)
        {
            if (string.IsNullOrEmpty(conditionName))
                return null;

            var normalized = conditionName.ToLowerInvariant().Trim();

            return normalized switch
            {
                "blinded" => Condition.Blinded,
                "charmed" => Condition.Charmed,
                "deafened" => Condition.Deafened,
                "frightened" => Condition.Frightened,
                "grappled" => Condition.Grappled,
                "incapacitated" => Condition.Incapacitated,
                "invisible" => Condition.Invisible,
                "paralyzed" => Condition.Paralyzed,
                "petrified" => Condition.Petrified,
                "poisoned" => Condition.Poisoned,
                "prone" => Condition.Prone,
                "restrained" => Condition.Restrained,
                "stunned" => Condition.Stunned,
                "unconscious" => Condition.Unconscious,
                "exhaustion" => Condition.Exhaustion,
                _ => Condition.None
            };
        }
    }
}