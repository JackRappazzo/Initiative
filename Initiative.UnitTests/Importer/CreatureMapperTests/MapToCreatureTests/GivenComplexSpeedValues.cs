using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenComplexSpeedValues : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasComplexSpeedValues)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldExtractNumberFromComplexSpeedObjects)
            .And(ShouldIgnoreConditionField);

        [Given]
        public void MonsterJsonHasComplexSpeedValues()
        {
            var complexWalkSpeed = """
            {
                "number": 15,
                "condition": "(30 ft. when rolling, 60 ft. rolling downhill)"
            }
            """;

            var complexFlySpeed = """
            {
                "number": 40,
                "condition": "(hover)"
            }
            """;

            MonsterJson = new MonsterJson
            {
                Name = "Test Creature",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("15").RootElement },
                HitPoints = new HitPointsJson { Average = 10 },
                Dexterity = 14,
                Speed = new SpeedJson
                {
                    Walk = JsonDocument.Parse(complexWalkSpeed).RootElement,
                    Fly = JsonDocument.Parse(complexFlySpeed).RootElement,
                    Swim = JsonDocument.Parse("30").RootElement, // Simple number for comparison
                    CanHover = false
                }
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldExtractNumberFromComplexSpeedObjects()
        {
            Assert.That(Result.WalkSpeed, Is.EqualTo(15), "Should extract number from complex walk speed object");
            Assert.That(Result.FlySpeed, Is.EqualTo(40), "Should extract number from complex fly speed object");
            Assert.That(Result.SwimSpeed, Is.EqualTo(30), "Should handle simple number speeds");
        }

        [Then]
        public void ShouldIgnoreConditionField()
        {
            // We're not storing condition information, just ensuring the numbers are extracted
            // The condition field should be ignored without causing errors
            Assert.That(Result.WalkSpeed, Is.Not.EqualTo(30), "Should not use condition values");
            Assert.That(Result.WalkSpeed, Is.Not.EqualTo(60), "Should not use condition values");
        }
    }
}