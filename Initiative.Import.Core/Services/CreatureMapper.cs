using Initiative.Import.Core.Models;
using Initiative.Import.Core.Services.Extractors;
using Initiative.Persistence.Models.Encounters;
using System.Text.Json;

namespace Initiative.Import.Core.Services
{
    /// <summary>
    /// Service to map imported JSON monster data to our internal Creature model
    /// </summary>
    public class CreatureMapper : ICreatureMapper
    {
        private readonly IArmorClassExtractor _armorClassExtractor;
        private readonly ISpeedExtractor _speedExtractor;
        private readonly IConditionImmunitiesExtractor _conditionImmunitiesExtractor;
        private readonly IDamageResistancesExtractor _damageResistancesExtractor;
        private readonly IActionsExtractor _actionsExtractor;
        private readonly IInitiativeCalculator _initiativeCalculator;
        private readonly ISystemNameGenerator _systemNameGenerator;
        private readonly ISpellcastingExtractor _spellcastingExtractor;

        public CreatureMapper(
            IArmorClassExtractor armorClassExtractor,
            ISpeedExtractor speedExtractor,
            IConditionImmunitiesExtractor conditionImmunitiesExtractor,
            IDamageResistancesExtractor damageResistancesExtractor,
            IActionsExtractor actionsExtractor,
            IInitiativeCalculator initiativeCalculator,
            ISystemNameGenerator systemNameGenerator,
            ISpellcastingExtractor spellcastingExtractor)
        {
            _armorClassExtractor = armorClassExtractor;
            _speedExtractor = speedExtractor;
            _conditionImmunitiesExtractor = conditionImmunitiesExtractor;
            _damageResistancesExtractor = damageResistancesExtractor;
            _actionsExtractor = actionsExtractor;
            _initiativeCalculator = initiativeCalculator;
            _systemNameGenerator = systemNameGenerator;
            _spellcastingExtractor = spellcastingExtractor;
        }

        /// <summary>
        /// Maps a MonsterJson to our internal Creature model
        /// </summary>
        /// <param name="monsterJson">The imported monster data</param>
        /// <returns>A Creature object for use in our system</returns>
        public Creature MapToCreature(MonsterJson monsterJson)
        {
            var charismaModifier = (monsterJson.Charisma - 10) / 2;
            var proficiencyBonus = CalculateProficiencyBonus(monsterJson.ChallengeRating);

            var creature = new Creature
            {
                Name = monsterJson.Name,
                SystemName = _systemNameGenerator.GenerateSystemName(monsterJson.Name),
                ArmorClass = _armorClassExtractor.ExtractArmorClass(monsterJson.ArmorClass),
                HitPoints = monsterJson.HitPoints?.Average ?? 1,
                MaximumHitPoints = monsterJson.HitPoints?.Average ?? 1,
                InitiativeModifier = _initiativeCalculator.CalculateInitiativeModifier(monsterJson.Dexterity, monsterJson.Initiative),
                Initiative = 0, // This will be rolled when added to an encounter
                Actions = _actionsExtractor.ExtractActions(monsterJson.Actions ?? new List<ActionJson>()),
                IsConcentrating = false,
                IsPlayer = false,
                
                // Map ability scores
                AbilityScores = new AbilityScores
                {
                    Strength = monsterJson.Strength,
                    Dexterity = monsterJson.Dexterity,
                    Constitution = monsterJson.Constitution,
                    Intelligence = monsterJson.Intelligence,
                    Wisdom = monsterJson.Wisdom,
                    Charisma = monsterJson.Charisma
                },
                
                // Map languages
                Languages = monsterJson.Languages ?? new List<string>(),
                
                // Map senses
                Senses = new Senses
                {
                    PassivePerception = monsterJson.PassivePerception
                }
            };

            // Extract speed values
            if (monsterJson.Speed != null)
            {
                creature.WalkSpeed = _speedExtractor.ExtractSpeedValue(monsterJson.Speed.Walk);
                creature.FlySpeed = _speedExtractor.ExtractSpeedValue(monsterJson.Speed.Fly);
                creature.SwimSpeed = _speedExtractor.ExtractSpeedValue(monsterJson.Speed.Swim);
                creature.BurrowSpeed = _speedExtractor.ExtractSpeedValue(monsterJson.Speed.Burrow);
                creature.ClimbSpeed = _speedExtractor.ExtractSpeedValue(monsterJson.Speed.Climb);
                creature.CanHover = monsterJson.Speed.CanHover ?? false;
            }

            // Extract and map condition immunities
            var conditionImmunityStrings = _conditionImmunitiesExtractor.ExtractConditionImmunities(monsterJson.ConditionImmunities);
            creature.ConditionImmunities = conditionImmunityStrings.ToPersistenceConditions();
            
            // Extract and map damage resistances, immunities, and vulnerabilities
            var damageResistances = _damageResistancesExtractor.ExtractDamageResistances(monsterJson.DamageResistances);
            var damageImmunities = _damageResistancesExtractor.ExtractDamageResistances(monsterJson.DamageImmunities);
            var damageVulnerabilities = _damageResistancesExtractor.ExtractDamageResistances(monsterJson.DamageVulnerabilities);
            
            creature.DamageResistances = damageResistances.ToPersistenceDamageTypes();
            creature.DamageImmunities = damageImmunities.ToPersistenceDamageTypes();
            creature.DamageVulnerabilities = damageVulnerabilities.ToPersistenceDamageTypes();

            // Extract spellcasting information
            var (isSpellcaster, spellcastingAbility, spellSaveDC, spellAttackBonus, spellSlots, spellsKnown) = 
                _spellcastingExtractor.ExtractSpellcasting(monsterJson.Spellcasting, charismaModifier, proficiencyBonus);
            
            creature.IsSpellcaster = isSpellcaster;
            creature.SpellcastingAbility = spellcastingAbility;
            creature.SpellSaveDC = spellSaveDC;
            creature.SpellAttackBonus = spellAttackBonus;
            creature.SpellSlots = spellSlots;
            creature.SpellsKnown = spellsKnown;
            
            return creature;
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

        /// <summary>
        /// Calculates proficiency bonus based on challenge rating
        /// </summary>
        /// <param name="challengeRatingElement">Challenge rating JSON element</param>
        /// <returns>Proficiency bonus</returns>
        private int CalculateProficiencyBonus(JsonElement challengeRatingElement)
        {
            // Try to extract CR value from either string or object format
            decimal cr = 0;
            
            if (challengeRatingElement.ValueKind == JsonValueKind.String)
            {
                var crString = challengeRatingElement.GetString();
                if (decimal.TryParse(crString, out cr))
                {
                    // Use parsed value
                }
                else if (crString == "1/8")
                {
                    cr = 0.125m;
                }
                else if (crString == "1/4")
                {
                    cr = 0.25m;
                }
                else if (crString == "1/2")
                {
                    cr = 0.5m;
                }
            }
            else if (challengeRatingElement.ValueKind == JsonValueKind.Object)
            {
                if (challengeRatingElement.TryGetProperty("cr", out var crProperty) && crProperty.ValueKind == JsonValueKind.String)
                {
                    var crString = crProperty.GetString();
                    if (decimal.TryParse(crString, out cr))
                    {
                        // Use parsed value
                    }
                    else if (crString == "1/8")
                    {
                        cr = 0.125m;
                    }
                    else if (crString == "1/4")
                    {
                        cr = 0.25m;
                    }
                    else if (crString == "1/2")
                    {
                        cr = 0.5m;
                    }
                }
            }

            // Calculate proficiency bonus based on CR
            return cr switch
            {
                >= 0 and < 5 => 2,
                >= 5 and < 9 => 3,
                >= 9 and < 13 => 4,
                >= 13 and < 17 => 5,
                >= 17 and < 21 => 6,
                >= 21 and < 25 => 7,
                >= 25 and < 29 => 8,
                >= 29 => 9,
                _ => 2
            };
        }
    }
}