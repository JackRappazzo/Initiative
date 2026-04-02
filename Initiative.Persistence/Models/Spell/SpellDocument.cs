using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Spell
{
    public class SpellDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        /// <summary>
        /// References SpellSourceDocument.Id.
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public required string SpellSourceId { get; set; }

        /// <summary>
        /// Denormalised source code (e.g. "XPHB") for convenient filtering without a join.
        /// </summary>
        public string? Source { get; set; }

        // ── Index / search fields (extracted at import time) ──────────────────

        public required string Name { get; set; }

        /// <summary>
        /// Full English school name, decoded from the 5etools single-letter code at import time.
        /// e.g. "Evocation", "Abjuration", "Necromancy".
        /// Null when the source omits the school field.
        /// </summary>
        public string? School { get; set; }

        // ── Full source data ──────────────────────────────────────────────────

        /// <summary>
        /// The complete 5etools spell JSON node stored verbatim as a BSON document.
        /// </summary>
        public required BsonDocument RawData { get; set; }
    }
}
