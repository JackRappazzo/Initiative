using Initiative.Import.Core.Models;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Interface for calculating initiative modifiers
    /// </summary>
    public interface IInitiativeCalculator
    {
        /// <summary>
        /// Calculates initiative modifier from dexterity and optional initiative proficiency
        /// </summary>
        /// <param name="dexterity">Dexterity score</param>
        /// <param name="initiativeData">Optional initiative proficiency data</param>
        /// <returns>Initiative modifier</returns>
        int CalculateInitiativeModifier(int dexterity, InitiativeJson? initiativeData);
    }
}