namespace Initiative.Api.Messages.Parties
{
    public class CreatePartyRequest
    {
        public required string Name { get; set; }
        public IEnumerable<PartyMemberJsonModel> Members { get; set; } = [];
    }
}
