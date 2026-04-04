namespace Initiative.Api.Messages
{
    public class FetchEncounterResponse
    {
        public required string EncounterId { get; set; }
               
        public required string DisplayName { get; set; }
               
        public IEnumerable<EncounterCreatureJsonModel> Creatures { get; set; }
               
        public DateTime CreatedAt { get; set; }

        public int TurnIndex { get; set; }

        public int TurnCount { get; set; }

        public bool ViewersAllowed { get; set; }

        public IEnumerable<int> PartyLevels { get; set; }

        public FetchEncounterResponse()
        {
            Creatures = new List<EncounterCreatureJsonModel>();
            PartyLevels = new List<int>();
        }
    }
}
