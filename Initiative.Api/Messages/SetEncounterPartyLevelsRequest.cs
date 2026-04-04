namespace Initiative.Api.Messages
{
    public class SetEncounterPartyLevelsRequest
    {
        public IEnumerable<int> PartyLevels { get; set; } = new List<int>();
    }
}
