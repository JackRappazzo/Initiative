using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Encounters
{
    public class EncounterCreature
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public bool IsPlayer { get; set; }

        public required string DisplayName { get; set; }

        public string? CreatureName { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? CreatureId { get; set; }

        public string? DndBeyondCharacterId { get; set; }

        public bool IsHpLinkedToDndBeyond { get; set; } = false;

        /// <summary>References the party member id when this creature is a sidekick.</summary>
        public string? SidekickId { get; set; }

        /// <summary>Rendered 5etools-style statblock snapshot for sidekicks.</summary>
        public BsonDocument? RawData { get; set; }

        public List<string> Statuses { get; set; } = new();

        public bool IsHidden { get; set; } = false;

        public int Initiative { get; set; }

        public int InitiativeModifier { get; set; }

        public int MaxHP { get; set; }

        public int CurrentHP { get; set; }

        public int AC { get; set; }
    }
}
