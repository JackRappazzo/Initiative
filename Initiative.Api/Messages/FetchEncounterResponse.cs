using Initiative.Persistence.Models.Encounters;

namespace Initiative.Api.Messages
{
    public class FetchEncounterResponse
    {
        public required string EncounterId { get; set; }
               
        public required string DisplayName { get; set; }
               
        public IEnumerable<CreatureJsonModel> Creatures { get; set; }
               
        public DateTime CreatedAt { get; set; }
        
        public FetchEncounterResponse()
        {
            Creatures = new List<CreatureJsonModel>();
        }
    }
}
