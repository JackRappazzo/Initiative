using Initiative.Persistence.Models.Parties;

namespace Initiative.Api.Core.Services.Sidekicks
{
    /// <summary>
    /// Encodes the Tasha's Cauldron of Everything sidekick progression rules.
    ///
    /// NOTE: Feature names/levels for the three classes are a best-effort transcription
    /// of TCE (pp. 142-147). The deterministic numbers below (hit dice, proficiency bonus,
    /// ability score increase levels, spellcaster slot/cantrip/spells-known tables) are the
    /// well-established values. Correct any feature text/levels here — everything flows
    /// from this single table.
    /// </summary>
    public static class SidekickRules
    {
        // ── General ────────────────────────────────────────────────────────────

        public static int ProficiencyBonus(int level) => level switch
        {
            >= 17 => 6,
            >= 13 => 5,
            >= 9 => 4,
            >= 5 => 3,
            _ => 2
        };

        public static readonly IReadOnlySet<int> AbilityScoreIncreaseLevels =
            new HashSet<int> { 4, 8, 12, 16, 19 };

        public static int HitDieMax(SidekickClass sidekickClass) => sidekickClass switch
        {
            SidekickClass.Spellcaster => 6,
            SidekickClass.Expert => 8,
            SidekickClass.Warrior => 10,
            _ => 8
        };

        /// <summary>Average hit points per hit die, rounded up (PHB fixed HP rule).</summary>
        public static int HitDieAverage(SidekickClass sidekickClass) => HitDieMax(sidekickClass) / 2 + 1;

        public static string HitDieSize(SidekickClass sidekickClass) => $"d{HitDieMax(sidekickClass)}";

        public static string SpellcastingAbility(SidekickSpellcasterKind kind) => kind switch
        {
            SidekickSpellcasterKind.Magician => "int",
            SidekickSpellcasterKind.Healer => "wis",
            SidekickSpellcasterKind.Prodigy => "cha",
            _ => "int"
        };

        // ── Spellcaster progression ─────────────────────────────────────────────

        public static int CantripsKnown(int level) => level switch
        {
            >= 10 => 4,
            >= 4 => 3,
            _ => 2
        };

        public static int SpellsKnown(int level) =>
            level <= 1 ? 1 : (level + 1) / 2 + 1;

        /// <summary>Spell slots by spell level (1-5). Mirrors the full-caster table capped at 5th.</summary>
        public static IReadOnlyDictionary<int, int> SpellSlots(int level)
        {
            var slots = new Dictionary<int, int>();

            void Add(int spellLevel, int count) { if (count > 0) slots[spellLevel] = count; }

            Add(1, level switch { 1 => 2, 2 => 3, _ => 4 });
            Add(2, level switch { 3 => 2, >= 4 => 3, _ => 0 });
            Add(3, level switch { 5 => 2, >= 6 => 3, _ => 0 });
            Add(4, level switch { 7 => 1, 8 => 2, >= 9 => 3, _ => 0 });
            Add(5, level switch { 9 => 1, >= 17 => 3, >= 10 => 2, _ => 0 });

            return slots;
        }

        // ── Class features ──────────────────────────────────────────────────────

        public static IEnumerable<SidekickFeature> FeaturesFor(SidekickClass sidekickClass, int level)
            => AllFeatures(sidekickClass).Where(f => f.Level <= level);

        private static IEnumerable<SidekickFeature> AllFeatures(SidekickClass sidekickClass) => sidekickClass switch
        {
            SidekickClass.Expert => ExpertFeatures,
            SidekickClass.Spellcaster => SpellcasterFeatures,
            SidekickClass.Warrior => WarriorFeatures,
            _ => Array.Empty<SidekickFeature>()
        };

        private static readonly SidekickFeature[] ExpertFeatures =
        {
            new(1, "Bonus Proficiencies",
                "The sidekick gains proficiency with light armor, one skill of your choice, and one saving throw of your choice.",
                SidekickChoiceType.Proficiencies),
            new(1, "Helpful",
                "The sidekick can take the Help action as a bonus action."),
            new(2, "Cunning Action",
                "On each of its turns, the sidekick can use a bonus action to take the Dash, Disengage, or Hide action."),
            new(3, "Expertise",
                "Choose two of the sidekick's skill proficiencies. The sidekick's proficiency bonus is doubled for any ability check it makes that uses either of the chosen proficiencies.",
                SidekickChoiceType.Expertise),
            new(6, "Coordinated Strike",
                "When the sidekick uses its Helpful feature to aid an ally's attack, the ally's attack deals an extra 1d6 damage on a hit."),
            new(7, "Evasion",
                "When the sidekick is subjected to an effect that allows a Dexterity saving throw to take only half damage, it instead takes no damage on a success and half damage on a failure."),
            new(11, "Reliable Talent",
                "The sidekick has refined its skills. Whenever it makes an ability check that lets it add its proficiency bonus, it can treat a d20 roll of 9 or lower as a 10."),
            new(17, "Coordinated Strike Improvement",
                "The extra damage of Coordinated Strike increases to 2d6."),
            new(18, "Expertise Improvement",
                "Choose two more of the sidekick's skill proficiencies to gain the benefits of Expertise.",
                SidekickChoiceType.Expertise),
            new(20, "Inspiring Help",
                "When the sidekick uses its Helpful feature, the creature receiving help also gains a +2 bonus to its next attack roll or ability check before the end of the sidekick's next turn."),
        };

        private static readonly SidekickFeature[] SpellcasterFeatures =
        {
            new(1, "Bonus Proficiencies",
                "The sidekick gains proficiency in one saving throw of your choice: Constitution, Intelligence, or Wisdom.",
                SidekickChoiceType.Proficiencies),
            new(1, "Spellcasting",
                "The sidekick casts spells using its chosen kind (Magician, Healer, or Prodigy) and its spellcasting ability.",
                SidekickChoiceType.SpellcasterKind),
            new(6, "Empowered Spells",
                "When the sidekick casts a spell, it can add its spellcasting ability modifier to one damage or healing roll of that spell."),
            new(18, "Focused Casting",
                "The sidekick has advantage on Constitution saving throws that it makes to maintain concentration on a spell."),
        };

        private static readonly SidekickFeature[] WarriorFeatures =
        {
            new(1, "Bonus Proficiencies",
                "The sidekick gains proficiency in all armor and shields, and with all simple and martial weapons, as well as in Strength and Constitution saving throws."),
            new(1, "Martial Role",
                "The sidekick chooses a martial role: Attacker or Defender.",
                SidekickChoiceType.WarriorRole),
            new(1, "Second Wind",
                "On its turn, the sidekick can use a bonus action to regain hit points equal to 1d10 + its level. Once it uses this feature, it must finish a short or long rest before using it again."),
            new(5, "Extra Attack",
                "The sidekick can attack twice, instead of once, whenever it takes the Attack action on its turn."),
            new(6, "Improved Critical",
                "The sidekick's weapon attacks score a critical hit on a roll of 19 or 20."),
            new(14, "Superior Critical",
                "The sidekick's weapon attacks score a critical hit on a roll of 18, 19, or 20."),
            new(20, "Second Attack",
                "The sidekick can attack three times, instead of twice, whenever it takes the Attack action on its turn."),
        };
    }

    public enum SidekickChoiceType
    {
        None,
        Proficiencies,
        Expertise,
        WarriorRole,
        SpellcasterKind,
        AbilityScoreIncrease,
        SpellSelection
    }

    public class SidekickFeature
    {
        public SidekickFeature(int level, string name, string description, SidekickChoiceType choiceType = SidekickChoiceType.None)
        {
            Level = level;
            Name = name;
            Description = description;
            ChoiceType = choiceType;
        }

        public int Level { get; }
        public string Name { get; }
        public string Description { get; }
        public SidekickChoiceType ChoiceType { get; }
    }
}
