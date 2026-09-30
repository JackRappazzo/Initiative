namespace Initiative.Api.Messages.Parties
{
    public class UpdatePartyMembersRequest
    {
        public IEnumerable<PartyMemberJsonModel> Members { get; set; } = [];
    }
}
