using System.Text.Json;
using System.Text.Json.Serialization;

namespace Initiative.Import.Core.Models
{
    /// <summary>
    /// Individual monster/creature data from JSON
    /// </summary>
    public class MonsterJson
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("page")]
        public int? Page { get; set; }

        [JsonPropertyName("size")]
        public List<string> Size { get; set; } = new();

        [JsonPropertyName("type")]
        public JsonElement Type { get; set; } // Can be string or object

        [JsonPropertyName("alignment")]
        public List<string> Alignment { get; set; } = new();

        [JsonPropertyName("ac")]
        public List<JsonElement> ArmorClass { get; set; } = new(); // Can be number or object

        [JsonPropertyName("hp")]
        public HitPointsJson? HitPoints { get; set; }

        [JsonPropertyName("speed")]
        public SpeedJson? Speed { get; set; }

        [JsonPropertyName("str")]
        public int Strength { get; set; }

        [JsonPropertyName("dex")]
        public int Dexterity { get; set; }

        [JsonPropertyName("con")]
        public int Constitution { get; set; }

        [JsonPropertyName("int")]
        public int Intelligence { get; set; }

        [JsonPropertyName("wis")]
        public int Wisdom { get; set; }

        [JsonPropertyName("cha")]
        public int Charisma { get; set; }

        [JsonPropertyName("save")]
        public Dictionary<string, string>? SavingThrows { get; set; }

        [JsonPropertyName("skill")]
        public Dictionary<string, string>? Skills { get; set; }

        [JsonPropertyName("senses")]
        public List<string>? Senses { get; set; }

        [JsonPropertyName("passive")]
        public int PassivePerception { get; set; }

        [JsonPropertyName("resist")]
        public List<string>? DamageResistances { get; set; }

        [JsonPropertyName("immune")]
        public List<string>? DamageImmunities { get; set; }

        [JsonPropertyName("vulnerable")]
        public List<string>? DamageVulnerabilities { get; set; }

        [JsonPropertyName("conditionImmune")]
        public List<string>? ConditionImmunities { get; set; }

        [JsonPropertyName("languages")]
        public List<string>? Languages { get; set; }

        [JsonPropertyName("cr")]
        public JsonElement ChallengeRating { get; set; } // Can be string or object

        [JsonPropertyName("initiative")]
        public InitiativeJson? Initiative { get; set; }

        [JsonPropertyName("trait")]
        public List<TraitJson>? Traits { get; set; }

        [JsonPropertyName("action")]
        public List<ActionJson>? Actions { get; set; }

        [JsonPropertyName("bonus")]
        public List<ActionJson>? BonusActions { get; set; }

        [JsonPropertyName("reaction")]
        public List<ActionJson>? Reactions { get; set; }

        [JsonPropertyName("legendary")]
        public List<ActionJson>? LegendaryActions { get; set; }

        [JsonPropertyName("spellcasting")]
        public List<SpellcastingJson>? Spellcasting { get; set; }

        [JsonPropertyName("environment")]
        public List<string>? Environment { get; set; }

        [JsonPropertyName("soundClip")]
        public SoundClipJson? SoundClip { get; set; }
    }
}