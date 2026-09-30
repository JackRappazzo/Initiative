using Initiative.Persistence.Models.Parties;

namespace Initiative.Api.Messages.Parties
{
    public class SaveSidekickRequest
    {
        public required string Name { get; set; }
        public required string BaseCreatureId { get; set; }
        public string? BaseCreatureName { get; set; }
        public string Class { get; set; } = "Expert";
        public int Level { get; set; } = 1;
        public SidekickAbilityScores AbilityScores { get; set; } = new();
        public List<SidekickHpRoll> HpRolls { get; set; } = new();
        public string? SpellcasterKind { get; set; }
        public string? WarriorRole { get; set; }
        public List<string> Cantrips { get; set; } = new();
        public Dictionary<int, List<string>> KnownSpells { get; set; } = new();
        public List<SidekickChoice> Choices { get; set; } = new();
    }
}
