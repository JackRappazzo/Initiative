using Initiative.Persistence.Models.Bestiary;

namespace Initiative.Api.Core.Services.Bestiary
{
    public class CustomCreatureAbilityScores
    {
        public int? Str { get; set; }
        public int? Dex { get; set; }
        public int? Con { get; set; }
        public int? Int { get; set; }
        public int? Wis { get; set; }
        public int? Cha { get; set; }
    }

    public class CustomCreatureSpeed
    {
        public int? Walk { get; set; }
        public int? Fly { get; set; }
        public int? Swim { get; set; }
        public int? Burrow { get; set; }
        public int? Climb { get; set; }
        public bool? CanHover { get; set; }
    }

    public class CustomCreatureEntry
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    public class CustomCreatureSpellcasting
    {
        public string? Ability { get; set; }
        public int? SpellSaveDc { get; set; }
        public int? SpellAttackBonus { get; set; }
        // Slot-based: key = "0" (cantrips) through "9"
        public Dictionary<string, List<string>>? SlotSpells { get; set; }
        // X/day format: each entry like "3/day: Fireball"
        public List<string>? DailySpells { get; set; }
        // Free-text description (markdown + {@spell} tags)
        public string? Description { get; set; }
        // Freeform spell list text (markdown + {@spell} tags); when set, SlotSpells/DailySpells are ignored
        public string? FreeformText { get; set; }
    }

    public class CustomCreatureData
    {
        public required string Name { get; set; }
        public string? Size { get; set; }
        public string? CreatureType { get; set; }
        public string? Subtype { get; set; }
        public string? Alignment { get; set; }
        public string? ChallengeRating { get; set; }
        public bool IsLegendary { get; set; }
        public int? ProficiencyBonus { get; set; }
        // HP: flat value and/or hit dice formula — both optional
        public int? HP { get; set; }
        public string? HitDice { get; set; }
        public int? AC { get; set; }
        public string? AcNote { get; set; }
        public CustomCreatureAbilityScores? AbilityScores { get; set; }
        public CustomCreatureSpeed? Speed { get; set; }
        // Saving throw overrides: key = ability shorthand (e.g. "str"), value = modifier string (e.g. "+5")
        public Dictionary<string, string>? SavingThrows { get; set; }
        // Skill overrides: key = skill name, value = modifier string
        public Dictionary<string, string>? Skills { get; set; }
        public List<string>? DamageResistances { get; set; }
        public List<string>? DamageImmunities { get; set; }
        public List<string>? DamageVulnerabilities { get; set; }
        public List<string>? ConditionImmunities { get; set; }
        public List<string>? Senses { get; set; }
        public List<string>? Languages { get; set; }
        public string? Traits { get; set; }
        public List<CustomCreatureEntry>? Actions { get; set; }
        public List<CustomCreatureEntry>? BonusActions { get; set; }
        public List<CustomCreatureEntry>? Reactions { get; set; }
        public List<CustomCreatureEntry>? LegendaryActions { get; set; }
        public int? LegendaryActionCount { get; set; }
        public CustomCreatureSpellcasting? Spellcasting { get; set; }
    }

    public interface IBestiaryService
    {
        Task<IEnumerable<BestiaryDocument>> GetAvailableBestiaries(string userId, CancellationToken cancellationToken);
        Task<SearchCreaturesResult> SearchCreatures(BestiarySearchQuery query, CancellationToken cancellationToken);
        Task<BestiaryCreatureDocument?> GetCreatureById(string creatureId, CancellationToken cancellationToken);

        Task<BestiaryDocument> CreateCustomBestiary(string userId, string name, CancellationToken cancellationToken);
        Task RenameBestiary(string bestiaryId, string userId, string name, CancellationToken cancellationToken);
        Task DeleteBestiary(string bestiaryId, string userId, CancellationToken cancellationToken);
        Task<BestiaryCreatureDocument> CreateCustomCreature(string bestiaryId, string userId, CustomCreatureData data, CancellationToken cancellationToken);
        Task UpdateCustomCreature(string creatureId, string bestiaryId, string userId, CustomCreatureData data, CancellationToken cancellationToken);
        Task DeleteCustomCreature(string creatureId, CancellationToken cancellationToken);
    }
}
