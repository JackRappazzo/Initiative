using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenSimpleSpeedValues : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasSimpleSpeedValues)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldMapAllSpeedValues)
            .And(ShouldSetCanHoverFlag);

        [Given]
        public void MonsterJsonHasSimpleSpeedValues()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Test Creature",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("15").RootElement },
                HitPoints = new HitPointsJson { Average = 10 },
                Dexterity = 14,
                Speed = new SpeedJson
                {
                    Walk = JsonDocument.Parse("30").RootElement,
                    Fly = JsonDocument.Parse("60").RootElement,
                    Swim = JsonDocument.Parse("20").RootElement,
                    Burrow = JsonDocument.Parse("15").RootElement,
                    Climb = JsonDocument.Parse("25").RootElement,
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
        public void ShouldMapAllSpeedValues()
        {
            Assert.That(Result.WalkSpeed, Is.EqualTo(30));
            Assert.That(Result.FlySpeed, Is.EqualTo(60));
            Assert.That(Result.SwimSpeed, Is.EqualTo(20));
            Assert.That(Result.BurrowSpeed, Is.EqualTo(15));
            Assert.That(Result.ClimbSpeed, Is.EqualTo(25));
        }

        [Then]
        public void ShouldSetCanHoverFlag()
        {
            Assert.That(Result.CanHover, Is.True);
        }
    }
}