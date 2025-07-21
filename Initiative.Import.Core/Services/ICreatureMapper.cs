using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using System.Text.Json;

namespace Initiative.Import.Core.Services
{
    public interface ICreatureMapper
    {
        Creature MapToCreature(MonsterJson monsterJson);
        List<Creature> MapToCreatures(IEnumerable<MonsterJson> monsters);
        
        /// <summary>
        /// Public method to extract condition immunities for testing purposes
        /// </summary>
        /// <param name="conditionImmunitiesList">List of JSON elements representing condition immunities</param>
        /// <returns>List of condition immunity names</returns>
        List<string> ExtractConditionImmunitiesPublic(List<JsonElement>? conditionImmunitiesList);
        
        /// <summary>
        /// Public method to extract damage resistances for testing purposes
        /// </summary>
        /// <param name="damageResistancesList">List of JSON elements representing damage resistances</param>
        /// <returns>List of damage resistance objects</returns>
        List<DamageResistanceJson> ExtractDamageResistancesPublic(List<JsonElement>? damageResistancesList);
    }
}