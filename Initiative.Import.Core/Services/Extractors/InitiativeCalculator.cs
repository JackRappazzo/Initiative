using Initiative.Import.Core.Models;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Service to calculate initiative modifiers
    /// </summary>
    public class InitiativeCalculator : IInitiativeCalculator
    {
        /// <summary>
        /// Calculates initiative modifier from dexterity and optional initiative proficiency
        /// </summary>
        /// <param name="dexterity">Dexterity score</param>
        /// <param name="initiativeData">Optional initiative proficiency data</param>
        /// <returns>Initiative modifier</returns>
        public int CalculateInitiativeModifier(int dexterity, InitiativeJson? initiativeData)
        {
            // Calculate base dexterity modifier
            int dexModifier = (dexterity - 10) / 2;

            // Add proficiency bonus if present
            int proficiencyBonus = initiativeData?.Proficiency ?? 0;

            return dexModifier + proficiencyBonus;
        }
    }
}