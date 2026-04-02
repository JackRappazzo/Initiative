using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Spell
{
    public class SpellSourceDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public required string Name { get; set; }

        /// <summary>
        /// Short source code as used in 5etools, e.g. "XPHB", "XGE", "TCE".
        /// </summary>
        public string? Source { get; set; }
    }
}
