using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenComplexConditionImmunities : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasComplexConditionImmunities)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldExtractConditionImmunitiesFromComplexObject);

        [Given]
        public void MonsterJsonHasComplexConditionImmunities()
        {
            // Create a complex condition immunity object like the one in the JSON example
            var complexConditionJson = """
            {
                "conditionImmune": [
                    "charmed"
                ],
                "note": "(with Mind Blank)",
                "cond": true
            }
            """;
            
            var conditionElements = new List<JsonElement>
            {
                JsonDocument.Parse(complexConditionJson).RootElement
            };

            MonsterJson = new MonsterJson
            {
                Name = "Archmage",
                ConditionImmunities = conditionElements,
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("17").RootElement },
                HitPoints = new HitPointsJson { Average = 170, Formula = "31d8+31" },
                Dexterity = 14
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldExtractConditionImmunitiesFromComplexObject()
        {
            // The mapper should successfully extract "charmed" from the complex object
            // and ignore the "note" and "cond" fields
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result.Name, Is.EqualTo("Archmage"));
        }
    }
}