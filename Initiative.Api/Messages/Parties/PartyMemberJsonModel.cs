namespace Initiative.Api.Messages.Parties
{
    public class PartyMemberJsonModel
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public int Level { get; set; }
        public string? DndBeyondCharacterId { get; set; }
        public SidekickJsonModel? Sidekick { get; set; }
    }
}
