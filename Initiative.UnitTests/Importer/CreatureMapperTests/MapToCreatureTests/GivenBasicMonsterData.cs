using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenBasicMonsterData : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonIsSetWithBasicData)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldMapBasicProperties)
            .And(ShouldGenerateSystemName)
            .And(ShouldCalculateInitiativeModifier)
            .And(ShouldSetDefaultValues)
            .And(ShouldHandleMissingSpeedData);

        [Given]
        public void MonsterJsonIsSetWithBasicData()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Goblin Warrior",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("15").RootElement },
                HitPoints = new HitPointsJson { Average = 7, Formula = "2d6" },
                Dexterity = 14, // Should give +2 modifier
                Initiative = null,
                Speed = null // No speed data provided
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldMapBasicProperties()
        {
            Assert.That(Result.Name, Is.EqualTo("Goblin Warrior"));
            Assert.That(Result.ArmorClass, Is.EqualTo(15));
            Assert.That(Result.HitPoints, Is.EqualTo(7));
            Assert.That(Result.MaximumHitPoints, Is.EqualTo(7));
        }

        [Then]
        public void ShouldGenerateSystemName()
        {
            Assert.That(Result.SystemName, Is.EqualTo("goblin-warrior"));
        }

        [Then]
        public void ShouldCalculateInitiativeModifier()
        {
            // Dex 14 = +2 modifier, no proficiency
            Assert.That(Result.InitiativeModifier, Is.EqualTo(2));
        }

        [Then]
        public void ShouldSetDefaultValues()
        {
            Assert.That(Result.Initiative, Is.EqualTo(0));
            Assert.That(Result.IsConcentrating, Is.False);
            Assert.That(Result.IsPlayer, Is.False);
        }

        [Then]
        public void ShouldHandleMissingSpeedData()
        {
            Assert.That(Result.WalkSpeed, Is.Null);
            Assert.That(Result.FlySpeed, Is.Null);
            Assert.That(Result.SwimSpeed, Is.Null);
            Assert.That(Result.BurrowSpeed, Is.Null);
            Assert.That(Result.ClimbSpeed, Is.Null);
            Assert.That(Result.CanHover, Is.False);
        }
    }
}