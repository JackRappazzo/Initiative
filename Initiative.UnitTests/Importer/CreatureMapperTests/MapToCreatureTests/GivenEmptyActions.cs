using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenEmptyActions : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasEmptyActions)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnEmptyActionsCollection);

        [Given]
        public void MonsterJsonHasEmptyActions()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Passive Creature",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("12").RootElement },
                HitPoints = new HitPointsJson { Average = 5, Formula = "1d8+1" },
                Dexterity = 10,
                Actions = new List<ActionJson>() // Empty list
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnEmptyActionsCollection()
        {
            Assert.That(Result.Actions, Is.Not.Null);
            Assert.That(Result.Actions, Is.Empty);
        }
    }
}