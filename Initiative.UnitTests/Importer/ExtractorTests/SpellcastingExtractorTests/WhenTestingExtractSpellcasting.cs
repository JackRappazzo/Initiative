using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpellcastingExtractorTests
{
    public abstract class WhenTestingExtractSpellcasting : WhenTestingSpellcastingExtractor
    {
        protected List<SpellcastingJson>? SpellcastingList;
        protected int CharismaModifier;
        protected int ProficiencyBonus;
        protected (bool IsSpellcaster, string? SpellcastingAbility, int? SpellSaveDC, int? SpellAttackBonus, Dictionary<int, int> SpellSlots, Dictionary<int, List<string>> SpellsKnown) Result;

        [When(DoNotRethrowExceptions: true)]
        public void ExtractSpellcastingIsCalled()
        {
            Result = SpellcastingExtractor.ExtractSpellcasting(SpellcastingList, CharismaModifier, ProficiencyBonus);
        }
    }
}