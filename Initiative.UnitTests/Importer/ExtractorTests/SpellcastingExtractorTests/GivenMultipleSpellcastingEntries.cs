using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpellcastingExtractorTests
{
    public class GivenMultipleSpellcastingEntries : WhenTestingExtractSpellcasting
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SpellcastingListHasMultipleEntries)
            .And(CharismaModifierIsSet)
            .And(ProficiencyBonusIsSet)
            .When(ExtractSpellcastingIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnSpellcaster)
            .And(ShouldMergeSpellsFromMultipleEntries)
            .And(ShouldUseLastSpellcastingAbility);

        [Given]
        public void SpellcastingListHasMultipleEntries()
        {
            SpellcastingList = new List<SpellcastingJson>
            {
                new SpellcastingJson
                {
                    Name = "Innate Spellcasting",
                    Type = "spellcasting",
                    SpellcastingAbility = "int",
                    HeaderEntries = new List<string>
                    {
                        "Spell save DC 15"
                    },
                    AtWill = new List<string>
                    {
                        "{@spell Detect Magic|XPHB}",
                        "{@spell Mage Hand|XPHB}"
                    }
                },
                new SpellcastingJson
                {
                    Name = "Spellcasting",
                    Type = "spellcasting",
                    SpellcastingAbility = "cha", // This should override the first one
                    Daily = new Dictionary<string, List<string>>
                    {
                        ["1e"] = new List<string>
                        {
                            "{@spell Charm Person|XPHB}"
                        }
                    }
                }
            };
        }

        [Given]
        public void CharismaModifierIsSet()
        {
            CharismaModifier = 3;
        }

        [Given]
        public void ProficiencyBonusIsSet()
        {
            ProficiencyBonus = 3;
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
        public void ShouldMergeSpellsFromMultipleEntries()
        {
            // Should have at-will spells from first entry
            Assert.That(Result.SpellsKnown.ContainsKey(0), Is.True);
            Assert.That(Result.SpellsKnown[0].Count, Is.EqualTo(2));
            Assert.That(Result.SpellsKnown[0], Contains.Item("Detect Magic"));
            Assert.That(Result.SpellsKnown[0], Contains.Item("Mage Hand"));

            // Should have daily spells from second entry
            Assert.That(Result.SpellsKnown.ContainsKey(1), Is.True);
            Assert.That(Result.SpellsKnown[1].Count, Is.EqualTo(1));
            Assert.That(Result.SpellsKnown[1], Contains.Item("Charm Person"));

            // Should have spell slots from second entry
            Assert.That(Result.SpellSlots.ContainsKey(1), Is.True);
            Assert.That(Result.SpellSlots[1], Is.EqualTo(1));
        }

        [Then]
        public void ShouldUseLastSpellcastingAbility()
        {
            // Should use the ability from the last entry
            Assert.That(Result.SpellcastingAbility, Is.EqualTo("cha"));
        }
    }
}