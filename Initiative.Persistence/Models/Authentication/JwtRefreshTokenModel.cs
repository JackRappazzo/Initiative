using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Authentication
{
    public class JwtRefreshTokenModel()
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set;}
        public string RefreshToken { get; set;}
        public DateTime Expiration { get; set; }
    }
}
