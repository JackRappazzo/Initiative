using System.Text.Json;
using Initiative.Import.Core.Models;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Interface for extracting damage resistances from JSON data
    /// </summary>
    public interface IDamageResistancesExtractor
    {
        /// <summary>
        /// Extracts damage resistances from JSON elements which can be strings or objects with special properties
        /// </summary>
        /// <param name="damageResistancesList">List of JSON elements representing damage resistances</param>
        /// <returns>List of damage resistance objects</returns>
        List<DamageResistanceJson> ExtractDamageResistances(List<JsonElement>? damageResistancesList);
    }
}