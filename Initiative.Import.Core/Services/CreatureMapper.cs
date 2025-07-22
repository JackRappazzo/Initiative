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

        public CreatureMapper(
            IArmorClassExtractor armorClassExtractor,
            ISpeedExtractor speedExtractor,
            IConditionImmunitiesExtractor conditionImmunitiesExtractor,
            IDamageResistancesExtractor damageResistancesExtractor,
            IActionsExtractor actionsExtractor,
            IInitiativeCalculator initiativeCalculator,
            ISystemNameGenerator systemNameGenerator)
        {
            _armorClassExtractor = armorClassExtractor;
            _speedExtractor = speedExtractor;
            _conditionImmunitiesExtractor = conditionImmunitiesExtractor;
            _damageResistancesExtractor = damageResistancesExtractor;
            _actionsExtractor = actionsExtractor;
            _initiativeCalculator = initiativeCalculator;
            _systemNameGenerator = systemNameGenerator;
        }

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
                SystemName = _systemNameGenerator.GenerateSystemName(monsterJson.Name),
                ArmorClass = _armorClassExtractor.ExtractArmorClass(monsterJson.ArmorClass),
                HitPoints = monsterJson.HitPoints?.Average ?? 1,
                MaximumHitPoints = monsterJson.HitPoints?.Average ?? 1,
                InitiativeModifier = _initiativeCalculator.CalculateInitiativeModifier(monsterJson.Dexterity, monsterJson.Initiative),
                Initiative = 0, // This will be rolled when added to an encounter
                Actions = _actionsExtractor.ExtractActions(monsterJson.Actions ?? new List<ActionJson>()),
                IsConcentrating = false,
                IsPlayer = false
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

            // Extract condition immunities for future use (currently not stored in Creature model)
            var conditionImmunities = _conditionImmunitiesExtractor.ExtractConditionImmunities(monsterJson.ConditionImmunities);
            
            // Extract damage resistances for future use (currently not stored in Creature model)
            var damageResistances = _damageResistancesExtractor.ExtractDamageResistances(monsterJson.DamageResistances);
            var damageImmunities = _damageResistancesExtractor.ExtractDamageResistances(monsterJson.DamageImmunities);
            var damageVulnerabilities = _damageResistancesExtractor.ExtractDamageResistances(monsterJson.DamageVulnerabilities);
            
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
    }
}