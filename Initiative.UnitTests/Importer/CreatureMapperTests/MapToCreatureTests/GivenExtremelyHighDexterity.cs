using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenExtremelyHighDexterity : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasVeryHighDexterity)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldCalculateHighInitiativeModifier);

        [Given]
        public void MonsterJsonHasVeryHighDexterity()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Lightning Quick Assassin",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("18").RootElement },
                HitPoints = new HitPointsJson { Average = 78, Formula = "12d8+24" },
                Dexterity = 30, // Should give +10 modifier (extreme case)
                Initiative = new InitiativeJson { Proficiency = 6 } // High level proficiency
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldCalculateHighInitiativeModifier()
        {
            // Dex 30 = +10 modifier, +6 proficiency = +16 total
            Assert.That(Result.InitiativeModifier, Is.EqualTo(16));
        }
    }
}