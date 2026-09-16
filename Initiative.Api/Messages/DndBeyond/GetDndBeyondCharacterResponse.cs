namespace Initiative.Api.Messages.DndBeyond
{
    public class GetDndBeyondCharacterResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Level { get; set; }
        public string? ClassName { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int TemporaryHP { get; set; }

        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        public int ArmorClass { get; set; }
        public int ProficiencyBonus { get; set; }
        public string? Race { get; set; }
        public int Speed { get; set; }

        public List<SpellSlot> SpellSlots { get; set; } = new();
        public List<SpellSlot> PactSlots { get; set; } = new();
        public List<PreparedSpell> PreparedSpells { get; set; } = new();
        public Resource? FocusPoints { get; set; }
        public List<Attack> Attacks { get; set; } = new();

        public class SpellSlot
        {
            public int Level { get; set; }
            public int Used { get; set; }
            public int Available { get; set; }
        }

        public class PreparedSpell
        {
            public string Name { get; set; } = "";
            public int Level { get; set; }
            public bool IsCantrip { get; set; }
        }

        public class Resource
        {
            public int Current { get; set; }
            public int Max { get; set; }
        }

        public class Attack
        {
            public string Name { get; set; } = "";
            public int ToHitBonus { get; set; }
            public string DamageDice { get; set; } = "";
            public int DamageBonus { get; set; }
            public string? DamageType { get; set; }
            public int Range { get; set; }
            public int LongRange { get; set; }
            public bool IsRanged { get; set; }
        }
    }
}
