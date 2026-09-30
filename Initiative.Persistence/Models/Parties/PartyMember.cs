namespace Initiative.Persistence.Models.Parties
{
    public class PartyMember
    {
        /// <summary>Stable identifier used to reference a member (e.g. a sidekick) for editing.</summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string Name { get; set; }
        public int Level { get; set; }
        public string? DndBeyondCharacterId { get; set; }

        /// <summary>Present when this member is a TCE sidekick; null for regular PCs.</summary>
        public Sidekick? Sidekick { get; set; }
    }
}
