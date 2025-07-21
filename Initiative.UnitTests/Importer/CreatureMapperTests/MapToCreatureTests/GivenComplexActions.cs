using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenComplexActions : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasComplexActions)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldMapAllComplexActions)
            .And(ShouldHandleVariableDescriptionCounts)
            .And(ShouldPreserveActionOrder);

        [Given]
        public void MonsterJsonHasComplexActions()
        {
            MonsterJson = new MonsterJson
            {
                Name = "Adult Red Dragon",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse("19").RootElement },
                HitPoints = new HitPointsJson { Average = 256, Formula = "19d12+133" },
                Dexterity = 10,
                Actions = new List<ActionJson>
                {
                    new ActionJson
                    {
                        Name = "Multiattack",
                        Entries = new List<string>
                        {
                            "The dragon can use its Frightful Presence. It then makes three attacks: one with its bite and two with its claws."
                        }
                    },
                    new ActionJson
                    {
                        Name = "Bite",
                        Entries = new List<string>
                        {
                            "Melee Weapon Attack: +17 to hit, reach 10 ft., one target.",
                            "Hit: 21 (2d10 + 10) piercing damage plus 14 (4d6) fire damage."
                        }
                    },
                    new ActionJson
                    {
                        Name = "Fire Breath (Recharge 5-6)",
                        Entries = new List<string>
                        {
                            "The dragon exhales fire in a 60-foot cone.",
                            "Each creature in that area must make a DC 21 Dexterity saving throw.",
                            "On a failed save, a creature takes 63 (18d6) fire damage.",
                            "On a successful save, the creature takes half as much damage."
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
        public void ShouldMapAllComplexActions()
        {
            Assert.That(Result.Actions, Is.Not.Null);
            Assert.That(Result.Actions.Count(), Is.EqualTo(3));
        }

        [Then]
        public void ShouldHandleVariableDescriptionCounts()
        {
            var multiattack = Result.Actions.FirstOrDefault(a => a.Name == "Multiattack");
            Assert.That(multiattack, Is.Not.Null);
            Assert.That(multiattack.Descriptions.Count(), Is.EqualTo(1));

            var bite = Result.Actions.FirstOrDefault(a => a.Name == "Bite");
            Assert.That(bite, Is.Not.Null);
            Assert.That(bite.Descriptions.Count(), Is.EqualTo(2));

            var fireBreath = Result.Actions.FirstOrDefault(a => a.Name == "Fire Breath (Recharge 5-6)");
            Assert.That(fireBreath, Is.Not.Null);
            Assert.That(fireBreath.Descriptions.Count(), Is.EqualTo(4));
        }

        [Then]
        public void ShouldPreserveActionOrder()
        {
            var actionsList = Result.Actions.ToList();
            Assert.That(actionsList[0].Name, Is.EqualTo("Multiattack"));
            Assert.That(actionsList[1].Name, Is.EqualTo("Bite"));
            Assert.That(actionsList[2].Name, Is.EqualTo("Fire Breath (Recharge 5-6)"));
        }
    }
}