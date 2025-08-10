using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpellcastingExtractorTests
{
    public class GivenInvalidDailySlotFormats : WhenTestingExtractSpellcasting
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SpellcastingListHasInvalidDailySlots)
            .And(CharismaModifierIsSet)
            .And(ProficiencyBonusIsSet)
            .When(ExtractSpellcastingIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnSpellcaster)
            .And(ShouldIgnoreInvalidSlotFormats)
            .And(ShouldStillExtractValidSpells);

        [Given]
        public void SpellcastingListHasInvalidDailySlots()
        {
            SpellcastingList = new List<SpellcastingJson>
            {
                new SpellcastingJson
                {
                    Name = "Spellcasting",
                    Type = "spellcasting",
                    SpellcastingAbility = "int",
                    HeaderEntries = new List<string>
                    {
                        "The creature has spell save DC 14"
                    },
                    Daily = new Dictionary<string, List<string>>
                    {
                        [""] = new List<string> // Empty key should be ignored
                        {
                            "{@spell Shield|XPHB}"
                        },
                        ["invalid"] = new List<string> // Invalid format should be ignored
                        {
                            "{@spell Magic Missile|XPHB}"
                        },
                        ["abc3"] = new List<string> // Invalid format should be ignored
                        {
                            "{@spell Fireball|XPHB}"
                        },
                        ["2e"] = new List<string> // Valid format should work
                        {
                            "{@spell Invisibility|XPHB}"
                        }
                    }
                }
            };
        }

        [Given]
        public void CharismaModifierIsSet()
        {
            CharismaModifier = 2;
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
        public void ShouldIgnoreInvalidSlotFormats()
        {
            // Should only have slot for level 2 (from "2e")
            Assert.That(Result.SpellSlots.Count, Is.EqualTo(1));
            Assert.That(Result.SpellSlots.ContainsKey(2), Is.True);
            Assert.That(Result.SpellSlots[2], Is.EqualTo(1));

            // Should not have slots for invalid formats
            Assert.That(Result.SpellSlots.ContainsKey(0), Is.False);
            Assert.That(Result.SpellSlots.ContainsKey(1), Is.False);
            Assert.That(Result.SpellSlots.ContainsKey(3), Is.False);
        }

        [Then]
        public void ShouldStillExtractValidSpells()
        {
            // Should still extract spells from valid level 2 entry
            Assert.That(Result.SpellsKnown.ContainsKey(2), Is.True);
            Assert.That(Result.SpellsKnown[2].Count, Is.EqualTo(1));
            Assert.That(Result.SpellsKnown[2], Contains.Item("Invisibility"));
        }
    }
}