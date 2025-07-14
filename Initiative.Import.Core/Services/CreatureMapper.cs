using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using System.Text.Json;

namespace Initiative.Import.Core.Services
{
    /// <summary>
    /// Service to map imported JSON monster data to our internal Creature model
    /// </summary>
    public class CreatureMapper : ICreatureMapper
    {
        /// <summary>
        /// Maps a MonsterJson to our internal Creature model
        /// </summary>
        /// <param name="monsterJson">The imported monster data</param>
        /// <returns>A Creature object for use in our system</returns>
        public Creature MapToCreature(MonsterJson monsterJson)
        {
            var creature = new Creature
            {
                Name = monsterJson.Name,
                SystemName = GenerateSystemName(monsterJson.Name),
                ArmorClass = ExtractArmorClass(monsterJson.ArmorClass),
                HitPoints = monsterJson.HitPoints?.Average ?? 1,
                MaximumHitPoints = monsterJson.HitPoints?.Average ?? 1,
                InitiativeModifier = CalculateInitiativeModifier(monsterJson.Dexterity, monsterJson.Initiative),
                Initiative = 0, // This will be rolled when added to an encounter
                IsConcentrating = false,
                IsPlayer = false
            };

            return creature;
        }

        /// <summary>
        /// Generates a system-friendly name from the creature name
        /// </summary>
        private string GenerateSystemName(string name)
        {
            return name.ToLowerInvariant()
                      .Replace(" ", "-")
                      .Replace("'", "")
                      .Replace(",", "")
                      .Replace("(", "")
                      .Replace(")", "");
        }

        /// <summary>
        /// Extracts armor class from the JSON element which can be a number or object
        /// </summary>
        private int ExtractArmorClass(List<JsonElement> armorClassList)
        {
            if (armorClassList == null || !armorClassList.Any())
                return 10; // Default AC

            var firstElement = armorClassList.First();

            if (firstElement.ValueKind == JsonValueKind.Number)
            {
                return firstElement.GetInt32();
            }
            else if (firstElement.ValueKind == JsonValueKind.Object)
            {
                // If it's an object, look for an "ac" property
                if (firstElement.TryGetProperty("ac", out var acProperty))
                {
                    return acProperty.GetInt32();
                }
            }

            return 10; // Default fallback
        }

        /// <summary>
        /// Calculates initiative modifier from dexterity and optional initiative proficiency
        /// </summary>
        private int CalculateInitiativeModifier(int dexterity, InitiativeJson? initiativeData)
        {
            // Calculate base dexterity modifier
            int dexModifier = (dexterity - 10) / 2;

            // Add proficiency bonus if present
            int proficiencyBonus = initiativeData?.Proficiency ?? 0;

            return dexModifier + proficiencyBonus;
        }

        /// <summary>
        /// Maps a list of MonsterJson objects to Creature objects
        /// </summary>
        /// <param name="monsters">List of imported monsters</param>
        /// <returns>List of Creature objects</returns>
        public List<Creature> MapToCreatures(IEnumerable<MonsterJson> monsters)
        {
            return monsters.Select(MapToCreature).ToList();
        }
    }
}