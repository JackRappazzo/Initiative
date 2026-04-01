namespace Initiative.Api.Messages.Parties
{
    public class FetchPartyResponse
    {
        public required string PartyId { get; set; }
        public required string Name { get; set; }
        public IEnumerable<PartyMemberJsonModel> Members { get; set; } = [];
    }
}
