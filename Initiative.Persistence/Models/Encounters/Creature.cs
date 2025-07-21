using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Initiative.Persistence.Models.Encounters
{
    public enum CreatureSize
    {
        Tiny,
        Small,
        Medium,
        Large,
        Huge,
        Gargantuan
    }

    public enum CreatureType
    {
        Aberration,
        Beast,
        Celestial,
        Construct,
        Dragon,
        Elemental,
        Fey,
        Fiend,
        Giant,
        Humanoid,
        Monstrosity,
        Ooze,
        Plant,
        Undead
    }

    public enum Alignment
    {
        LawfulGood,
        NeutralGood,
        ChaoticGood,
        LawfulNeutral,
        TrueNeutral,
        ChaoticNeutral,
        LawfulEvil,
        NeutralEvil,
        ChaoticEvil,
        Unaligned
    }

    public enum DamageType
    {
        Acid,
        Bludgeoning,
        Cold,
        Fire,
        Force,
        Lightning,
        Necrotic,
        Piercing,
        Poison,
        Psychic,
        Radiant,
        Slashing,
        Thunder
    }

    public enum Skill
    {
        Acrobatics,
        AnimalHandling,
        Arcana,
        Athletics,
        Deception,
        History,
        Insight,
        Intimidation,
        Investigation,
        Medicine,
        Nature,
        Perception,
        Performance,
        Persuasion,
        Religion,
        SleightOfHand,
        Stealth,
        Survival
    }

    public enum Condition
    {
        Blinded,
        Charmed,
        Deafened,
        Frightened,
        Grappled,
        Incapacitated,
        Invisible,
        Paralyzed,
        Petrified,
        Poisoned,
        Prone,
        Restrained,
        Stunned,
        Unconscious,
        Exhaustion
    }

    public enum SenseType
    {
        Blindsight,
        Darkvision,
        Tremorsense,
        Truesight
    }

    public class AbilityScores
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        private int GetModifier(int score) => (score - 10) / 2;
        public int StrengthModifier => GetModifier(Strength);
        public int DexterityModifier => GetModifier(Dexterity);
        public int ConstitutionModifier => GetModifier(Constitution);
        public int IntelligenceModifier => GetModifier(Intelligence);
        public int WisdomModifier => GetModifier(Wisdom);
        public int CharismaModifier => GetModifier(Charisma);
    }

    public class Speed
    {
        public int Walk { get; set; }
        public int? Fly { get; set; }
        public int? Swim { get; set; }
        public int? Climb { get; set; }
        public int? Burrow { get; set; }
        public bool? Hover { get; set; }
    }

    public class Senses
    {
        public Dictionary<SenseType, int> SpecialSenses { get; set; } = new();
        public int PassivePerception { get; set; }
    }

    public class Creature
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        // Basic Information
        public string Name { get; set; }
        public string SystemName { get; set; }
        public CreatureSize Size { get; set; }
        public CreatureType Type { get; set; }
        public string? Subtype { get; set; }
        public Alignment Alignment { get; set; }
        
        // Combat Stats
        public int Initiative { get; set; }
        public int InitiativeModifier { get; set; }
        public int HitPoints { get; set; }
        public int MaximumHitPoints { get; set; }
        public string? HitDice { get; set; }
        public int ArmorClass { get; set; }
        public string? ArmorDescription { get; set; }
        
        // Ability Scores
        public AbilityScores AbilityScores { get; set; } = new();
        
        // Movement
        public Speed Speed { get; set; } = new();
        
        // Senses and Perception
        public Senses Senses { get; set; } = new();
        
        // Languages
        public List<string> Languages { get; set; } = new();
        
        // Challenge Rating and Experience
        public decimal ChallengeRating { get; set; }
        public int ExperiencePoints { get; set; }
        public int ProficiencyBonus { get; set; }
        
        // Saving Throws (only include if proficient)
        public Dictionary<string, int> SavingThrowProficiencies { get; set; } = new();
        
        // Skills (only include if proficient or expert)
        public Dictionary<Skill, int> SkillProficiencies { get; set; } = new();
        
        // Damage Resistances, Immunities, and Vulnerabilities
        public List<DamageType> DamageResistances { get; set; } = new();
        public List<DamageType> DamageImmunities { get; set; } = new();
        public List<DamageType> DamageVulnerabilities { get; set; } = new();
        
        // Condition Immunities
        public List<Condition> ConditionImmunities { get; set; } = new();
        
        // Special Abilities and Features
        public List<string> SpecialAbilities { get; set; } = new();
        public List<string> Actions { get; set; } = new();
        public List<string> BonusActions { get; set; } = new();
        public List<string> Reactions { get; set; } = new();
        public List<string> LegendaryActions { get; set; } = new();
        public int? LegendaryActionCount { get; set; }
        public List<string> LairActions { get; set; } = new();
        public List<string> RegionalEffects { get; set; } = new();
        
        // Spellcasting
        public bool IsSpellcaster { get; set; }
        public string? SpellcastingAbility { get; set; }
        public int? SpellSaveDC { get; set; }
        public int? SpellAttackBonus { get; set; }
        public Dictionary<int, int> SpellSlots { get; set; } = new(); // Level -> Slots
        public Dictionary<int, List<string>> SpellsKnown { get; set; } = new(); // Level -> Spell names
        
        // Initiative Tracker specific properties
        public bool IsConcentrating { get; set; }
        public bool IsPlayer { get; set; }

        // Speed properties
        public int? WalkSpeed { get; set; }
        public int? FlySpeed { get; set; }
        public int? SwimSpeed { get; set; }
        public int? BurrowSpeed { get; set; }
        public int? ClimbSpeed { get; set; }
        public bool CanHover { get; set; }

        //Actions and spells
        public IEnumerable<CreatureAction> Actions { get; set; } = new List<CreatureAction>();


        public bool IsPlayer { get; set; }
        public string? Notes { get; set; }
        public List<string> CurrentConditions { get; set; } = new();
        
        // Optional: Source information
        public string? Source { get; set; }
        public string? SourcePage { get; set; }
    }
}
