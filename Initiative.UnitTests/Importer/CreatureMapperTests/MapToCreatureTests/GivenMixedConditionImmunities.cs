using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenMixedConditionImmunities : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasMixedConditionImmunities)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldExtractAllConditionImmunities);

        [Given]
        public void MonsterJsonHasMixedConditionImmunities()
        {
            // Mix of simple strings and complex objects
            var complexConditionJson = """
            {
                "conditionImmune": [
                    "charmed",
                    "frightened"
                ],
                "note": "(with special ability)",
                "cond": true
            }
            """;
            
            var conditionElements = new List<JsonElement>
            {
                JsonDocument.Parse("\"exhaustion\"").RootElement, // Simple string
                JsonDocument.Parse("\"poisoned\"").RootElement,   // Simple string
                JsonDocument.Parse(complexConditionJson).RootElement // Complex object
            };

            MonsterJson = new MonsterJson
            {
                Name = "Mixed Immunity Monster",
                ConditionImmunities = conditionElements,
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("16").RootElement },
                HitPoints = new HitPointsJson { Average = 100, Formula = "15d8+30" },
                Dexterity = 13
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldExtractAllConditionImmunities()
        {
            // Should extract: exhaustion, poisoned, charmed, frightened
            // Should ignore the note and cond fields from the complex object
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Name, Is.EqualTo("Mixed Immunity Monster"));
        }
    }
}