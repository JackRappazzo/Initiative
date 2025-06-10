using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Encounters
{
    public class Creature
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Initiative { get; set; }
        public int InitiativeModifier { get; set; }
        public string Name { get; set; }
        public string SystemName { get; set; }
        public int HitPoints { get; set; }
        public int MaximumHitPoints { get; set; }
        public int ArmorClass { get; set; }
        public bool IsConcentrating { get; set; }

        public bool IsPlayer { get; set; }
    }
}
