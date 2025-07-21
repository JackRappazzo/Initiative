using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenActionsWithEmptyEntries : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasActionsWithEmptyEntries)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldMapActionsWithEmptyDescriptions);

        [Given]
        public void MonsterJsonHasActionsWithEmptyEntries()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Incomplete Creature",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("14").RootElement },
                HitPoints = new HitPointsJson { Average = 8, Formula = "2d6+1" },
                Dexterity = 12,
                Actions = new List<ActionJson>
                {
                    new ActionJson
                    {
                        Name = "Incomplete Action",
                        Entries = new List<string>() // Empty entries
                    },
                    new ActionJson
                    {
                        Name = "Normal Action",
                        Entries = new List<string>
                        {
                            "This action has a description."
                        }
                    }
                }
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldMapActionsWithEmptyDescriptions()
        {
            Assert.That(Result.Actions, Is.Not.Null);
            Assert.That(Result.Actions.Count(), Is.EqualTo(2));

            var incompleteAction = Result.Actions.FirstOrDefault(a => a.Name == "Incomplete Action");
            Assert.That(incompleteAction, Is.Not.Null);
            Assert.That(incompleteAction.Descriptions, Is.Not.Null);
            Assert.That(incompleteAction.Descriptions, Is.Empty);

            var normalAction = Result.Actions.FirstOrDefault(a => a.Name == "Normal Action");
            Assert.That(normalAction, Is.Not.Null);
            Assert.That(normalAction.Descriptions.Count(), Is.EqualTo(1));
            Assert.That(normalAction.Descriptions.First(), Is.EqualTo("This action has a description."));
        }
    }
}