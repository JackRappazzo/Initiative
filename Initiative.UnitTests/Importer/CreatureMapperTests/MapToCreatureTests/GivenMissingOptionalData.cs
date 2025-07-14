using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenMissingOptionalData : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasMissingOptionalFields)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldUseDefaultValues);

        [Given]
        public void MonsterJsonHasMissingOptionalFields()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Simple Creature",
                ArmorClass = null, // Missing AC
                HitPoints = null, // Missing HP
                Dexterity = 10, // Neutral dexterity
                Initiative = null // No initiative proficiency
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldUseDefaultValues()
        {
            Assert.That(Result.ArmorClass, Is.EqualTo(10)); // Default AC
            Assert.That(Result.HitPoints, Is.EqualTo(1)); // Default HP
            Assert.That(Result.MaximumHitPoints, Is.EqualTo(1)); // Default max HP
            Assert.That(Result.InitiativeModifier, Is.EqualTo(0)); // Dex 10 = 0 modifier
        }
    }
}