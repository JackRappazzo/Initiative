using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpellcastingExtractorTests
{
    public class GivenSpellcastingWithoutHeaderEntries : WhenTestingExtractSpellcasting
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SpellcastingListHasNoHeaderEntries)
            .And(CharismaModifierIsSet)
            .And(ProficiencyBonusIsSet)
            .When(ExtractSpellcastingIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnSpellcaster)
            .And(ShouldCalculateDefaultSpellSaveDC)
            .And(ShouldCalculateDefaultSpellAttackBonus)
            .And(ShouldExtractSpells);

        [Given]
        public void SpellcastingListHasNoHeaderEntries()
        {
            SpellcastingList = new List<SpellcastingJson>
            {
                new SpellcastingJson
                {
                    Name = "Spellcasting",
                    Type = "spellcasting",
                    SpellcastingAbility = "cha",
                    AtWill = new List<string>
                    {
                        "{@spell Light|XPHB}",
                        "Prestidigitation"
                    }
                }
            };
        }

        [Given]
        public void CharismaModifierIsSet()
        {
            CharismaModifier = 3; // 16 Charisma
        }

        [Given]
        public void ProficiencyBonusIsSet()
        {
            ProficiencyBonus = 2; // Low-level creature
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
        public void ShouldCalculateDefaultSpellSaveDC()
        {
            // 8 + proficiency bonus (2) + charisma modifier (3) = 13
            Assert.That(Result.SpellSaveDC, Is.EqualTo(13));
        }

        [Then]
        public void ShouldCalculateDefaultSpellAttackBonus()
        {
            // proficiency bonus (2) + charisma modifier (3) = 5
            Assert.That(Result.SpellAttackBonus, Is.EqualTo(5));
        }

        [Then]
        public void ShouldExtractSpells()
        {
            Assert.That(Result.SpellsKnown.ContainsKey(0), Is.True);
            Assert.That(Result.SpellsKnown[0].Count, Is.EqualTo(2));
            Assert.That(Result.SpellsKnown[0], Contains.Item("Light"));
            Assert.That(Result.SpellsKnown[0], Contains.Item("Prestidigitation"));
        }
    }
}