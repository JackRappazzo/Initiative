using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenNullActions : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasNullActions)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldHandleNullActionsGracefully);

        [Given]
        public void MonsterJsonHasNullActions()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Simple Creature",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("10").RootElement },
                HitPoints = new HitPointsJson { Average = 3, Formula = "1d6" },
                Dexterity = 8,
                Actions = null // Null actions
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldHandleNullActionsGracefully()
        {
            Assert.That(Result.Actions, Is.Not.Null);
            Assert.That(Result.Actions, Is.Empty, "Actions should be empty when null actions are provided");
        }
    }
}