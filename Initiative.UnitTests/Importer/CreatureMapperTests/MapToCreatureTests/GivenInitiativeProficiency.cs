using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenInitiativeProficiency : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasInitiativeProficiency)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldAddProficiencyToInitiativeModifier);

        [Given]
        public void MonsterJsonHasInitiativeProficiency()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Alert Rogue",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("15").RootElement },
                HitPoints = new HitPointsJson { Average = 32, Formula = "5d8+10" },
                Dexterity = 16, // Should give +3 modifier
                Initiative = new InitiativeJson { Proficiency = 2 } // +2 proficiency
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldAddProficiencyToInitiativeModifier()
        {
            // Dex 16 = +3 modifier, +2 proficiency = +5 total
            Assert.That(Result.InitiativeModifier, Is.EqualTo(5));
        }
    }
}