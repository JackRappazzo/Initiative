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
                Actions = ExtractActions(monsterJson.Actions ?? new List<ActionJson>()),
                IsConcentrating = false,
                IsPlayer = false
            };

            // Extract speed values
            if (monsterJson.Speed != null)
            {
                creature.WalkSpeed = ExtractSpeedValue(monsterJson.Speed.Walk);
                creature.FlySpeed = ExtractSpeedValue(monsterJson.Speed.Fly);
                creature.SwimSpeed = ExtractSpeedValue(monsterJson.Speed.Swim);
                creature.BurrowSpeed = ExtractSpeedValue(monsterJson.Speed.Burrow);
                creature.ClimbSpeed = ExtractSpeedValue(monsterJson.Speed.Climb);
                creature.CanHover = monsterJson.Speed.CanHover ?? false;
            }

            // Extract condition immunities for future use (currently not stored in Creature model)
            var conditionImmunities = ExtractConditionImmunities(monsterJson.ConditionImmunities);
            
            // Extract damage resistances for future use (currently not stored in Creature model)
            var damageResistances = ExtractDamageResistances(monsterJson.DamageResistances);
            var damageImmunities = ExtractDamageResistances(monsterJson.DamageImmunities);
            var damageVulnerabilities = ExtractDamageResistances(monsterJson.DamageVulnerabilities);
            
            return creature;
        }

        /// <summary>
        /// Public method to extract condition immunities for testing purposes
        /// </summary>
        /// <param name="conditionImmunitiesList">List of JSON elements representing condition immunities</param>
        /// <returns>List of condition immunity names</returns>
        public List<string> ExtractConditionImmunitiesPublic(List<JsonElement>? conditionImmunitiesList)
        {
            return ExtractConditionImmunities(conditionImmunitiesList);
        }

        /// <summary>
        /// Public method to extract damage resistances for testing purposes
        /// </summary>
        /// <param name="damageResistancesList">List of JSON elements representing damage resistances</param>
        /// <returns>List of damage resistance objects</returns>
        public List<DamageResistanceJson> ExtractDamageResistancesPublic(List<JsonElement>? damageResistancesList)
        {
            return ExtractDamageResistances(damageResistancesList);
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
        /// Extracts speed value from JSON element which can be a number or object with number/condition
        /// </summary>
        /// <param name="speedElement">JSON element representing speed (can be number or object)</param>
        /// <returns>Speed value or null if not present</returns>
        private int? ExtractSpeedValue(JsonElement? speedElement)
        {
            if (speedElement == null || !speedElement.HasValue)
                return null;

            var element = speedElement.Value;

            if (element.ValueKind == JsonValueKind.Number)
            {
                return element.GetInt32();
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                // If it's an object, look for a "number" property
                if (element.TryGetProperty("number", out var numberProperty))
                {
                    return numberProperty.GetInt32();
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts condition immunities from JSON elements which can be strings or objects
        /// </summary>
        /// <param name="conditionImmunitiesList">List of JSON elements representing condition immunities</param>
        /// <returns>List of condition immunity names</returns>
        private List<string> ExtractConditionImmunities(List<JsonElement>? conditionImmunitiesList)
        {
            if (conditionImmunitiesList == null || !conditionImmunitiesList.Any())
                return new List<string>();

            var conditions = new List<string>();

            foreach (var element in conditionImmunitiesList)
            {
                if (element.ValueKind == JsonValueKind.String)
                {
                    // Simple string condition
                    conditions.Add(element.GetString() ?? string.Empty);
                }
                else if (element.ValueKind == JsonValueKind.Object)
                {
                    // Complex object with conditionImmune array
                    if (element.TryGetProperty("conditionImmune", out var conditionProperty) &&
                        conditionProperty.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var condition in conditionProperty.EnumerateArray())
                        {
                            if (condition.ValueKind == JsonValueKind.String)
                            {
                                conditions.Add(condition.GetString() ?? string.Empty);
                            }
                        }
                    }
                }
            }

            return conditions.Where(c => !string.IsNullOrEmpty(c)).ToList();
        }

        /// <summary>
        /// Extracts damage resistances from JSON elements which can be strings or objects with special properties
        /// </summary>
        /// <param name="damageResistancesList">List of JSON elements representing damage resistances</param>
        /// <returns>List of damage resistance objects</returns>
        private List<DamageResistanceJson> ExtractDamageResistances(List<JsonElement>? damageResistancesList)
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


        private IEnumerable<CreatureAction> ExtractActions(List<ActionJson> actions)
        {
            if((actions?.Count ?? 0 ) == 0)
                return Enumerable.Empty<CreatureAction>();
            return actions.Select(a => new CreatureAction()
            {
                Name = a.Name,
                Descriptions = a.Entries.ToArray()
            });
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