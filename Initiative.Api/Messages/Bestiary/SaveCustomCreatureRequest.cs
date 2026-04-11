namespace Initiative.Api.Messages.Bestiary
{
    public class SaveCustomCreatureAbilityScores
    {
        public int? Str { get; set; }
        public int? Dex { get; set; }
        public int? Con { get; set; }
        public int? Int { get; set; }
        public int? Wis { get; set; }
        public int? Cha { get; set; }
    }

    public class SaveCustomCreatureSpeed
    {
        public int? Walk { get; set; }
        public int? Fly { get; set; }
        public int? Swim { get; set; }
        public int? Burrow { get; set; }
        public int? Climb { get; set; }
        public bool? CanHover { get; set; }
    }

    public class SaveCustomCreatureEntry
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    public class SaveCustomCreatureSpellcasting
    {
        public string? Ability { get; set; }
        public int? SpellSaveDc { get; set; }
        public int? SpellAttackBonus { get; set; }
        public Dictionary<string, List<string>>? SlotSpells { get; set; }
        public List<string>? DailySpells { get; set; }
        /// <summary>Free-text description shown before the spell list (markdown + {@spell} tags supported).</summary>
        public string? Description { get; set; }
        /// <summary>Freeform spell list text (markdown + {@spell} tags). When set, SlotSpells/DailySpells are ignored.</summary>
        public string? FreeformText { get; set; }
    }

    public class SaveCustomCreatureRequest
    {
        public required string Name { get; set; }
        public string? Size { get; set; }
        public string? CreatureType { get; set; }
        public string? Subtype { get; set; }
        public string? Alignment { get; set; }
        public string? ChallengeRating { get; set; }
        public bool IsLegendary { get; set; }
        public int? ProficiencyBonus { get; set; }
        public int? HP { get; set; }
        public string? HitDice { get; set; }
        public int? AC { get; set; }
        public string? AcNote { get; set; }
        public SaveCustomCreatureAbilityScores? AbilityScores { get; set; }
        public SaveCustomCreatureSpeed? Speed { get; set; }
        public Dictionary<string, string>? SavingThrows { get; set; }
        public Dictionary<string, string>? Skills { get; set; }
        public List<string>? DamageResistances { get; set; }
        public List<string>? DamageImmunities { get; set; }
        public List<string>? DamageVulnerabilities { get; set; }
        public List<string>? ConditionImmunities { get; set; }
        public List<string>? Senses { get; set; }
        public List<string>? Languages { get; set; }
        public string? Traits { get; set; }
        public List<SaveCustomCreatureEntry>? Actions { get; set; }
        public List<SaveCustomCreatureEntry>? BonusActions { get; set; }
        public List<SaveCustomCreatureEntry>? Reactions { get; set; }
        public List<SaveCustomCreatureEntry>? LegendaryActions { get; set; }
        public int? LegendaryActionCount { get; set; }
        public SaveCustomCreatureSpellcasting? Spellcasting { get; set; }
    }
}
