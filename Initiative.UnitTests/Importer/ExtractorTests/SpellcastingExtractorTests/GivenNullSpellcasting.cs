using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SpellcastingExtractorTests
{
    public class GivenNullSpellcasting : WhenTestingExtractSpellcasting
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SpellcastingListIsNull)
            .And(CharismaModifierIsSet)
            .And(ProficiencyBonusIsSet)
            .When(ExtractSpellcastingIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnNonSpellcaster)
            .And(ShouldReturnEmptyCollections);

        [Given]
        public void SpellcastingListIsNull()
        {
            SpellcastingList = null;
        }

        [Given]
        public void CharismaModifierIsSet()
        {
            CharismaModifier = 4; // 18 Charisma
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
        public void ShouldReturnNonSpellcaster()
        {
            Assert.That(Result.IsSpellcaster, Is.False);
            Assert.That(Result.SpellcastingAbility, Is.Null);
            Assert.That(Result.SpellSaveDC, Is.Null);
            Assert.That(Result.SpellAttackBonus, Is.Null);
        }

        [Then]
        public void ShouldReturnEmptyCollections()
        {
            Assert.That(Result.SpellSlots, Is.Not.Null);
            Assert.That(Result.SpellSlots, Is.Empty);
            Assert.That(Result.SpellsKnown, Is.Not.Null);
            Assert.That(Result.SpellsKnown, Is.Empty);
        }
    }
}