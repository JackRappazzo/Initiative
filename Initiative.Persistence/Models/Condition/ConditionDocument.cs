using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Condition
{
    public class ConditionDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Source code from the JSON record (e.g. "XPHB").
        /// </summary>
        public required string Source { get; set; }

        /// <summary>
        /// Type of entry: "condition" or "status".
        /// </summary>
        public required string Type { get; set; }

        // ── Index / search fields (extracted at import time) ──────────────────

        public required string Name { get; set; }

        // ── Raw data for detailed rendering ──────────────────────────────────

        /// <summary>
        /// The full condition/status JSON object from 5etools, stored as-is for flexible rendering.
        /// Includes entries, page references, reprints, and other metadata.
        /// </summary>
        public required BsonDocument RawData { get; set; }
    }
}
