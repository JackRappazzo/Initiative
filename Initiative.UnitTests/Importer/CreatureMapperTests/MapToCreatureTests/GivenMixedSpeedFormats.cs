using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenMixedSpeedFormats : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasMixedSpeedFormats)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldHandleBothSimpleAndComplexSpeeds)
            .And(ShouldHandlePartialSpeedData);

        [Given]
        public void MonsterJsonHasMixedSpeedFormats()
        {
            var complexFlySpeed = """
            {
                "number": 90,
                "condition": "(hover)"
            }
            """;

            MonsterJson = new MonsterJson
            {
                Name = "Mixed Speed Creature",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("15").RootElement },
                HitPoints = new HitPointsJson { Average = 10 },
                Dexterity = 14,
                Speed = new SpeedJson
                {
                    Walk = JsonDocument.Parse("40").RootElement, // Simple number
                    Fly = JsonDocument.Parse(complexFlySpeed).RootElement, // Complex object
                    Swim = JsonDocument.Parse("40").RootElement, // Simple number
                    // Burrow and Climb are null (not provided)
                    CanHover = true
                }
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldHandleBothSimpleAndComplexSpeeds()
        {
            Assert.That(Result.WalkSpeed, Is.EqualTo(40), "Should handle simple walk speed");
            Assert.That(Result.FlySpeed, Is.EqualTo(90), "Should extract number from complex fly speed");
            Assert.That(Result.SwimSpeed, Is.EqualTo(40), "Should handle simple swim speed");
            Assert.That(Result.CanHover, Is.True, "Should preserve CanHover flag");
        }

        [Then]
        public void ShouldHandlePartialSpeedData()
        {
            Assert.That(Result.BurrowSpeed, Is.Null, "Should handle missing burrow speed");
            Assert.That(Result.ClimbSpeed, Is.Null, "Should handle missing climb speed");
        }
    }
}