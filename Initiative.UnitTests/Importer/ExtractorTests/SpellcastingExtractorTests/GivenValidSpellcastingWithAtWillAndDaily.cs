using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpellcastingExtractorTests
{
    public class GivenValidSpellcastingWithAtWillAndDaily : WhenTestingExtractSpellcasting
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SpellcastingListContainsValidData)
            .And(CharismaModifierIsSet)
            .And(ProficiencyBonusIsSet)
            .When(ExtractSpellcastingIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnSpellcaster)
            .And(ShouldExtractSpellcastingAbility)
            .And(ShouldExtractSpellSaveDC)
            .And(ShouldExtractAtWillSpells)
            .And(ShouldExtractDailySpells)
            .And(ShouldExtractSpellSlots);

        [Given]
        public void SpellcastingListContainsValidData()
        {
            SpellcastingList = new List<SpellcastingJson>
            {
                new SpellcastingJson
                {
                    Name = "Spellcasting",
                    Type = "spellcasting",
                    SpellcastingAbility = "cha",
                    HeaderEntries = new List<string>
                    {
                        "The dragon casts one of the following spells, requiring no Material components and using Charisma as the spellcasting ability (spell save {@dc 17}):"
                    },
                    AtWill = new List<string>
                    {
                        "{@spell Detect Magic|XPHB}",
                        "{@spell Mind Spike|XPHB} (level 4 version)",
                        "{@spell Minor Illusion|XPHB}"
                    },
                    Daily = new Dictionary<string, List<string>>
                    {
                        ["1e"] = new List<string>
                        {
                            "{@spell Greater Restoration|XPHB}",
                            "{@spell Major Image|XPHB}"
                        }
                    }
                }
            };
        }

        [Given]
        public void CharismaModifierIsSet()
        {
            CharismaModifier = 4; // 18 Charisma
        }

        [Given]
        public void ProficiencyBonusIsSet()
        {
            ProficiencyBonus = 5; // CR 14 creature
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
        public void ShouldExtractSpellcastingAbility()
        {
            Assert.That(Result.SpellcastingAbility, Is.EqualTo("cha"));
        }

        [Then]
        public void ShouldExtractSpellSaveDC()
        {
            Assert.That(Result.SpellSaveDC, Is.EqualTo(17));
        }

        [Then]
        public void ShouldExtractAtWillSpells()
        {
            Assert.That(Result.SpellsKnown.ContainsKey(0), Is.True);
            Assert.That(Result.SpellsKnown[0].Count, Is.EqualTo(3));
            Assert.That(Result.SpellsKnown[0], Contains.Item("Detect Magic"));
            Assert.That(Result.SpellsKnown[0], Contains.Item("Mind Spike"));
            Assert.That(Result.SpellsKnown[0], Contains.Item("Minor Illusion"));
        }

        [Then]
        public void ShouldExtractDailySpells()
        {
            Assert.That(Result.SpellsKnown.ContainsKey(1), Is.True);
            Assert.That(Result.SpellsKnown[1].Count, Is.EqualTo(2));
            Assert.That(Result.SpellsKnown[1], Contains.Item("Greater Restoration"));
            Assert.That(Result.SpellsKnown[1], Contains.Item("Major Image"));
        }

        [Then]
        public void ShouldExtractSpellSlots()
        {
            Assert.That(Result.SpellSlots.ContainsKey(1), Is.True);
            Assert.That(Result.SpellSlots[1], Is.EqualTo(1)); // "1e" means 1 slot each
        }
    }
}