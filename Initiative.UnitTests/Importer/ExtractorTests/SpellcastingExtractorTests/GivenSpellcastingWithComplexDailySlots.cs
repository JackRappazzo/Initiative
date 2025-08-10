using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpellcastingExtractorTests
{
    public class GivenSpellcastingWithComplexDailySlots : WhenTestingExtractSpellcasting
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SpellcastingListHasComplexDailySlots)
            .And(CharismaModifierIsSet)
            .And(ProficiencyBonusIsSet)
            .When(ExtractSpellcastingIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnSpellcaster)
            .And(ShouldExtractSpellAttackBonus)
            .And(ShouldParseComplexDailySlots)
            .And(ShouldExtractSpellsFromMultipleLevels);

        [Given]
        public void SpellcastingListHasComplexDailySlots()
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
                        "The wizard is an 18th-level spellcaster with spell attack +11:",
                        "Spell save DC 19"
                    },
                    Daily = new Dictionary<string, List<string>>
                    {
                        ["1e"] = new List<string>
                        {
                            "{@spell Shield|XPHB}",
                            "{@spell Magic Missile|XPHB}"
                        },
                        ["3e"] = new List<string>
                        {
                            "{@spell Fireball|XPHB}",
                            "{@spell Lightning Bolt|XPHB}"
                        },
                        ["5"] = new List<string>
                        {
                            "{@spell Cone of Cold|XPHB}"
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
            ProficiencyBonus = 6;
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
        public void ShouldExtractSpellAttackBonus()
        {
            Assert.That(Result.SpellAttackBonus, Is.EqualTo(11));
        }

        [Then]
        public void ShouldParseComplexDailySlots()
        {
            Assert.That(Result.SpellSlots.ContainsKey(1), Is.True);
            Assert.That(Result.SpellSlots[1], Is.EqualTo(1)); // "1e" = 1 slot each

            Assert.That(Result.SpellSlots.ContainsKey(3), Is.True);
            Assert.That(Result.SpellSlots[3], Is.EqualTo(1)); // "3e" = 1 slot each

            Assert.That(Result.SpellSlots.ContainsKey(5), Is.True);
            Assert.That(Result.SpellSlots[5], Is.EqualTo(5)); // "5" = 5 slots
        }

        [Then]
        public void ShouldExtractSpellsFromMultipleLevels()
        {
            // Level 1 spells
            Assert.That(Result.SpellsKnown.ContainsKey(1), Is.True);
            Assert.That(Result.SpellsKnown[1].Count, Is.EqualTo(2));
            Assert.That(Result.SpellsKnown[1], Contains.Item("Shield"));
            Assert.That(Result.SpellsKnown[1], Contains.Item("Magic Missile"));

            // Level 3 spells
            Assert.That(Result.SpellsKnown.ContainsKey(3), Is.True);
            Assert.That(Result.SpellsKnown[3].Count, Is.EqualTo(2));
            Assert.That(Result.SpellsKnown[3], Contains.Item("Fireball"));
            Assert.That(Result.SpellsKnown[3], Contains.Item("Lightning Bolt"));

            // Level 5 spells
            Assert.That(Result.SpellsKnown.ContainsKey(5), Is.True);
            Assert.That(Result.SpellsKnown[5].Count, Is.EqualTo(1));
            Assert.That(Result.SpellsKnown[5], Contains.Item("Cone of Cold"));
        }
    }
}