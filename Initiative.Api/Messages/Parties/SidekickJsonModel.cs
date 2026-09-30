using Initiative.Persistence.Models.Parties;

namespace Initiative.Api.Messages.Parties
{
    public class SidekickJsonModel
    {
        public required string BaseCreatureId { get; set; }
        public required string BaseCreatureName { get; set; }
        public required string Class { get; set; }
        public int Level { get; set; }
        public SidekickAbilityScores AbilityScores { get; set; } = new();
        public int MaxHitPoints { get; set; }
        public int HitDiceCount { get; set; }
        public string HitDieSize { get; set; } = "d8";
        public IEnumerable<SidekickHpRoll> HpRolls { get; set; } = [];
        public SidekickSpellcastingJsonModel? Spellcasting { get; set; }
        public IEnumerable<SidekickChoice> Choices { get; set; } = [];
        public object RawData { get; set; } = new object();
    }

    public class SidekickSpellcastingJsonModel
    {
        public string Kind { get; set; } = "Magician";
        public string Ability { get; set; } = "int";
        public List<string> Cantrips { get; set; } = new();
        public Dictionary<int, int> Slots { get; set; } = new();
        public Dictionary<int, List<string>> KnownSpells { get; set; } = new();
    }
}
