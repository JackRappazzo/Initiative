using AspNetCore.Identity.Mongo.Model;

namespace Initiative.Api.Core.Identity
{
    public class InitiativeUser : MongoUser
    {
        public string DisplayName { get; set; } 
        public bool IsActive { get; set; }
        public string CurrentRoomCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
