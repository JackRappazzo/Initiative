using Initiative.Persistence.Models.Parties;

namespace Initiative.Api.Core.Services.Sidekicks
{
    /// <summary>Client-supplied sidekick state; the service resolves the base creature and builds the stored model.</summary>
    public class SidekickData
    {
        public required string BaseCreatureId { get; set; }
        public string? BaseCreatureName { get; set; }
        public SidekickClass Class { get; set; }
        public int Level { get; set; }
        public SidekickAbilityScores AbilityScores { get; set; } = new();
        public List<SidekickHpRoll> HpRolls { get; set; } = new();
        public SidekickSpellcasterKind? SpellcasterKind { get; set; }
        public SidekickWarriorRole? WarriorRole { get; set; }
        public List<string> Cantrips { get; set; } = new();
        public Dictionary<int, List<string>> KnownSpells { get; set; } = new();
        public List<SidekickChoice> Choices { get; set; } = new();
    }
}
