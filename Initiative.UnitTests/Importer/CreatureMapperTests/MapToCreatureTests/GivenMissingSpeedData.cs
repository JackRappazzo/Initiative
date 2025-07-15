using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenMissingSpeedData : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasNoSpeedData)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldSetSpeedValuesToNull)
            .And(ShouldSetCanHoverToFalse);

        [Given]
        public void MonsterJsonHasNoSpeedData()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Test Creature",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("15").RootElement },
                HitPoints = new HitPointsJson { Average = 10 },
                Dexterity = 14,
                Speed = null // No speed data
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldSetSpeedValuesToNull()
        {
            Assert.That(Result.WalkSpeed, Is.Null);
            Assert.That(Result.FlySpeed, Is.Null);
            Assert.That(Result.SwimSpeed, Is.Null);
            Assert.That(Result.BurrowSpeed, Is.Null);
            Assert.That(Result.ClimbSpeed, Is.Null);
        }

        [Then]
        public void ShouldSetCanHoverToFalse()
        {
            Assert.That(Result.CanHover, Is.False);
        }
    }
}