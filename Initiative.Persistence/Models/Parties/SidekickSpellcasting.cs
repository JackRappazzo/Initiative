using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Parties
{
    public class SidekickSpellcasting
    {
        public SidekickSpellcasterKind Kind { get; set; }

        /// <summary>Spellcasting ability shorthand: "str", "dex", "con", "int", "wis", "cha".</summary>
        public string Ability { get; set; } = "int";

        public List<string> Cantrips { get; set; } = new();

        /// <summary>Spell slots: spell level (1-5) → available slots.</summary>
        [BsonRepresentation(BsonType.Array)]
        public Dictionary<int, int> Slots { get; set; } = new();

        /// <summary>Known/prepared spells: spell level → spell names.</summary>
        [BsonRepresentation(BsonType.Array)]
        public Dictionary<int, List<string>> KnownSpells { get; set; } = new();
    }
}
