namespace Initiative.Persistence.Models.Parties
{
    public class SidekickAbilityScores
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        private static int Modifier(int score) => (int)Math.Floor((score - 10) / 2.0);

        public int StrengthModifier => Modifier(Strength);
        public int DexterityModifier => Modifier(Dexterity);
        public int ConstitutionModifier => Modifier(Constitution);
        public int IntelligenceModifier => Modifier(Intelligence);
        public int WisdomModifier => Modifier(Wisdom);
        public int CharismaModifier => Modifier(Charisma);
    }
}
