namespace Initiative.Persistence.Models.Parties
{
    /// <summary>
    /// A recorded decision made while leveling a sidekick, e.g. Expertise skills,
    /// Warrior role, Spellcaster kind, an ability score increase, or spell selections.
    /// </summary>
    public class SidekickChoice
    {
        public int Level { get; set; }

        /// <summary>Machine-readable choice type, e.g. "Expertise", "WarriorRole", "AbilityScoreIncrease".</summary>
        public required string Type { get; set; }

        /// <summary>Optional single value (e.g. ability shorthand "str", "con").</summary>
        public string? Value { get; set; }

        /// <summary>Optional list of values (e.g. skill names, spell names).</summary>
        public List<string> Values { get; set; } = new();
    }
}
