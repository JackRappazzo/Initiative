using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Encounters
{
    public class Encounter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DisplayName { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; }

        public DateTime CreatedAt { get; set; }

        public IEnumerable<EncounterCreature> Creatures { get; set; }

        public int TurnIndex { get; set; }

        public int TurnCount { get; set; }

        public bool ViewersAllowed { get; set; }

    }
}
