using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Initiative.Persistence.Models
{
    public class InitiativeUserModel
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string IdentityId { get; set; }
        public string EmailAddress { get; set; }
        public string RoomCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoggedInAt { get; set; }
    }
}
