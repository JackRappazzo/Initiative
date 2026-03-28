using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Bestiary
{
    public class BestiaryCreatureDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        /// <summary>
        /// References BestiaryDocument.Id.
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public required string BestiaryId { get; set; }

        /// <summary>
        /// Denormalised source code (e.g. "MM") for convenient filtering without a join.
        /// </summary>
        public required string Source { get; set; }

        // ── Index / search fields (extracted at import time) ──────────────────

        public required string Name { get; set; }

        /// <summary>
        /// Primary creature type as a string, e.g. "humanoid", "dragon", "undead".
        /// Derived from the 5etools <c>type</c> field (string) or <c>type.type</c> (object form).
        /// Null when the source uses a "choose" structure with no single primary type.
        /// </summary>
        public string? CreatureType { get; set; }

        /// <summary>
        /// Challenge rating as a normalised string, e.g. "0", "1/8", "1/4", "1/2", "1" … "30".
        /// Derived from the 5etools <c>cr</c> field (string) or <c>cr.cr</c> (object form).
        /// Null when the source omits CR entirely.
        /// </summary>
        public string? ChallengeRating { get; set; }

        /// <summary>
        /// True when the creature has at least one legendary action or a legendary actions header.
        /// Derived at import time from 5etools <c>legendary</c> / <c>legendaryActions</c> fields.
        /// </summary>
        public bool IsLegendary { get; set; }

        // ── Full source data ──────────────────────────────────────────────────

        /// <summary>
        /// The complete 5etools creature JSON node stored verbatim as a BSON document.
        /// Used to render the statblock on the frontend and to support future rich queries.
        /// </summary>
        public required BsonDocument RawData { get; set; }
    }
}
