using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Bestiary
{
    public class BestiaryDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public required string Name { get; set; }

        /// <summary>
        /// Short source code as used in 5etools, e.g. "MM", "PHB", "XGE".
        /// </summary>
        public required string Source { get; set; }

        /// <summary>
        /// True for bestiaries imported from 5etools source files.
        /// False for future user-created bestiaries.
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Null for system bestiaries. Set to the owner's user ID for user bestiaries.
        /// </summary>
        public string? OwnerId { get; set; }
    }
}
