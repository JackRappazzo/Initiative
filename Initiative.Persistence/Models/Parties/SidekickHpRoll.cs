namespace Initiative.Persistence.Models.Parties
{
    public class SidekickHpRoll
    {
        public int Level { get; set; }

        /// <summary>True when the fixed average was taken; false when the die was rolled.</summary>
        public bool IsFixed { get; set; }

        /// <summary>Raw die result (or the average value when fixed), before the CON modifier.</summary>
        public int DieValue { get; set; }

        public int ConstitutionModifier { get; set; }

        /// <summary>Total HP gained for this level (DieValue + ConstitutionModifier).</summary>
        public int Result { get; set; }
    }
}
