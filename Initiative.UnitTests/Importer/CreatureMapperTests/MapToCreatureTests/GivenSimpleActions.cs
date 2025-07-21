using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenSimpleActions : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasSimpleActions)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldMapAllActions)
            .And(ShouldMapActionNames)
            .And(ShouldMapActionDescriptions);

        [Given]
        public void MonsterJsonHasSimpleActions()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Goblin",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("15").RootElement },
                HitPoints = new HitPointsJson { Average = 7, Formula = "2d6" },
                Dexterity = 14,
                Actions = new List<ActionJson>
                {
                    new ActionJson
                    {
                        Name = "Scimitar",
                        Entries = new List<string>
                        {
                            "Melee Weapon Attack: +4 to hit, reach 5 ft., one target.",
                            "Hit: 5 (1d6 + 2) slashing damage."
                        }
                    },
                    new ActionJson
                    {
                        Name = "Shortbow",
                        Entries = new List<string>
                        {
                            "Ranged Weapon Attack: +4 to hit, range 80/320 ft., one target.",
                            "Hit: 5 (1d6 + 2) piercing damage."
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
        public void ShouldMapAllActions()
        {
            Assert.That(Result.Actions, Is.Not.Null);
            Assert.That(Result.Actions.Count(), Is.EqualTo(2));
        }

        [Then]
        public void ShouldMapActionNames()
        {
            var actionsList = Result.Actions.ToList();
            Assert.That(actionsList[0].Name, Is.EqualTo("Scimitar"));
            Assert.That(actionsList[1].Name, Is.EqualTo("Shortbow"));
        }

        [Then]
        public void ShouldMapActionDescriptions()
        {
            var scimitarAction = Result.Actions.FirstOrDefault(a => a.Name == "Scimitar");
            Assert.That(scimitarAction, Is.Not.Null);
            Assert.That(scimitarAction.Descriptions.Count(), Is.EqualTo(2));
            Assert.That(scimitarAction.Descriptions.First(), Is.EqualTo("Melee Weapon Attack: +4 to hit, reach 5 ft., one target."));
            Assert.That(scimitarAction.Descriptions.Last(), Is.EqualTo("Hit: 5 (1d6 + 2) slashing damage."));

            var shortbowAction = Result.Actions.FirstOrDefault(a => a.Name == "Shortbow");
            Assert.That(shortbowAction, Is.Not.Null);
            Assert.That(shortbowAction.Descriptions.Count(), Is.EqualTo(2));
            Assert.That(shortbowAction.Descriptions.First(), Is.EqualTo("Ranged Weapon Attack: +4 to hit, range 80/320 ft., one target."));
            Assert.That(shortbowAction.Descriptions.Last(), Is.EqualTo("Hit: 5 (1d6 + 2) piercing damage."));
        }
    }
}