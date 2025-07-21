using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenSingleActionWithMultipleDescriptions : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasSingleActionWithMultipleDescriptions)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldMapSingleAction)
            .And(ShouldMapAllDescriptions);

        [Given]
        public void MonsterJsonHasSingleActionWithMultipleDescriptions()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Wolf",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("13").RootElement },
                HitPoints = new HitPointsJson { Average = 11, Formula = "2d8+2" },
                Dexterity = 15,
                Actions = new List<ActionJson>
                {
                    new ActionJson
                    {
                        Name = "Bite",
                        Entries = new List<string>
                        {
                            "Melee Weapon Attack: +4 to hit, reach 5 ft., one target.",
                            "Hit: 7 (2d4 + 2) piercing damage.",
                            "If the target is a creature, it must succeed on a DC 11 Strength saving throw or be knocked prone."
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
        public void ShouldMapSingleAction()
        {
            Assert.That(Result.Actions, Is.Not.Null);
            Assert.That(Result.Actions.Count(), Is.EqualTo(1));
        }

        [Then]
        public void ShouldMapAllDescriptions()
        {
            var biteAction = Result.Actions.First();
            Assert.That(biteAction.Name, Is.EqualTo("Bite"));
            Assert.That(biteAction.Descriptions.Count(), Is.EqualTo(3));
            Assert.That(biteAction.Descriptions.First(), Is.EqualTo("Melee Weapon Attack: +4 to hit, reach 5 ft., one target."));
            Assert.That(biteAction.Descriptions.ElementAt(1), Is.EqualTo("Hit: 7 (2d4 + 2) piercing damage."));
            Assert.That(biteAction.Descriptions.Last(), Is.EqualTo("If the target is a creature, it must succeed on a DC 11 Strength saving throw or be knocked prone."));
        }
    }
}