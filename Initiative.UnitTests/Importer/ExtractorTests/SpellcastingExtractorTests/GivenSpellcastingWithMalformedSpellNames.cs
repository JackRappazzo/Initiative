using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpellcastingExtractorTests
{
    public class GivenSpellcastingWithMalformedSpellNames : WhenTestingExtractSpellcasting
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SpellcastingListHasMalformedSpellNames)
            .And(CharismaModifierIsSet)
            .And(ProficiencyBonusIsSet)
            .When(ExtractSpellcastingIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnSpellcaster)
            .And(ShouldCleanUpMalformedSpellNames)
            .And(ShouldIgnoreEmptySpellNames);

        [Given]
        public void SpellcastingListHasMalformedSpellNames()
        {
            SpellcastingList = new List<SpellcastingJson>
            {
                new SpellcastingJson
                {
                    Name = "Spellcasting",
                    Type = "spellcasting",
                    SpellcastingAbility = "wis",
                    AtWill = new List<string>
                    {
                        "{@spell Cure Wounds", // Missing closing brace
                        "Healing Word}", // Missing opening
                        "", // Empty string
                        "{@spell Sacred Flame|XPHB}", // Properly formatted
                        "Light", // Simple name
                        "   ", // Whitespace only
                        "{@spell Hold Person|XPHB} (3rd level)" // With additional text
                    }
                }
            };
        }

        [Given]
        public void CharismaModifierIsSet()
        {
            CharismaModifier = 1;
        }

        [Given]
        public void ProficiencyBonusIsSet()
        {
            ProficiencyBonus = 2;
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnSpellcaster()
        {
            Assert.That(Result.IsSpellcaster, Is.True);
        }

        [Then]
        public void ShouldCleanUpMalformedSpellNames()
        {
            Assert.That(Result.SpellsKnown.ContainsKey(0), Is.True);
            var spells = Result.SpellsKnown[0];
            
            // Should clean up malformed entries
            Assert.That(spells, Contains.Item("Cure Wounds"));
            Assert.That(spells, Contains.Item("Healing Word"));
            Assert.That(spells, Contains.Item("Sacred Flame"));
            Assert.That(spells, Contains.Item("Light"));
            Assert.That(spells, Contains.Item("Hold Person"));
        }

        [Then]
        public void ShouldIgnoreEmptySpellNames()
        {
            var spells = Result.SpellsKnown[0];
            
            // Should not contain empty or whitespace-only entries
            Assert.That(spells.Any(s => string.IsNullOrWhiteSpace(s)), Is.False);
            
            // Should have exactly 5 valid spells
            Assert.That(spells.Count, Is.EqualTo(5));
        }
    }
}