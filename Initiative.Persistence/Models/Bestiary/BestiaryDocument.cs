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
        /// Null for user-created custom bestiaries.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Null for system bestiaries. Set to the owner's user ID for user-created bestiaries.
        /// </summary>
        public string? OwnerId { get; set; }
    }
}
