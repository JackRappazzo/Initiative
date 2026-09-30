using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Parties
{
    public class Sidekick
    {
        /// <summary>References BestiaryCreatureDocument.Id for the base creature.</summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public required string BaseCreatureId { get; set; }

        /// <summary>Denormalised base creature name for convenient display without a join.</summary>
        public required string BaseCreatureName { get; set; }

        public SidekickClass Class { get; set; }

        public int Level { get; set; }

        public SidekickAbilityScores AbilityScores { get; set; } = new();

        public int MaxHitPoints { get; set; }

        public int HitDiceCount { get; set; }

        /// <summary>Hit die size as a string, e.g. "d6", "d8", "d10".</summary>
        public string HitDieSize { get; set; } = "d8";

        /// <summary>Per-level HP gain log, used to reconcile when de-leveling or re-leveling.</summary>
        public List<SidekickHpRoll> HpRolls { get; set; } = new();

        public SidekickSpellcasting? Spellcasting { get; set; }

        /// <summary>Recorded decisions made while leveling (expertise, role, ASI, spell picks, etc.).</summary>
        public List<SidekickChoice> Choices { get; set; } = new();

        /// <summary>Generated 5etools-style statblock snapshot used for rendering.</summary>
        public required BsonDocument RawData { get; set; }
    }
}
