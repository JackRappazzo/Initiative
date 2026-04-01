using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Parties
{
    public class PartyDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public required string Name { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public required string OwnerId { get; set; }

        public IEnumerable<PartyMember> Members { get; set; } = [];
    }
}
