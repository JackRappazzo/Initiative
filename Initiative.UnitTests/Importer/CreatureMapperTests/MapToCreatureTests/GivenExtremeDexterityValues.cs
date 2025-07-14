using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenExtremeDexterityValues : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasVeryLowDexterity)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldCalculateNegativeInitiativeModifier);

        [Given]
        public void MonsterJsonHasVeryLowDexterity()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Slow Zombie",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("8").RootElement },
                HitPoints = new HitPointsJson { Average = 22, Formula = "3d8+9" },
                Dexterity = 6, // Should give -2 modifier
                Initiative = null
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldCalculateNegativeInitiativeModifier()
        {
            // Dex 6 = -2 modifier
            Assert.That(Result.InitiativeModifier, Is.EqualTo(-2));
        }
    }
}