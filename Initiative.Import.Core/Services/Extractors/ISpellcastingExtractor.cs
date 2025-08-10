using Initiative.Import.Core.Models;

namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Interface for extracting spellcasting information from JSON data
    /// </summary>
    public interface ISpellcastingExtractor
    {
        /// <summary>
        /// Extracts spellcasting information from a list of SpellcastingJson objects
        /// </summary>
        /// <param name="spellcastingList">List of spellcasting JSON objects</param>
        /// <returns>Tuple containing spellcasting details: (IsSpellcaster, SpellcastingAbility, SpellSaveDC, SpellAttackBonus, SpellSlots, SpellsKnown)</returns>
        (bool IsSpellcaster, string? SpellcastingAbility, int? SpellSaveDC, int? SpellAttackBonus, Dictionary<int, int> SpellSlots, Dictionary<int, List<string>> SpellsKnown) ExtractSpellcasting(List<SpellcastingJson>? spellcastingList, int charismaModifier, int proficiencyBonus);
    }
}