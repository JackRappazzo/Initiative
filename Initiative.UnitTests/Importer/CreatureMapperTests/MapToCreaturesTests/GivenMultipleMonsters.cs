using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreaturesTests
{
    public class GivenMultipleMonsters : WhenTestingMapToCreatures
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonstersListContainsMultipleMonsters)
            .When(MapToCreaturesIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnCorrectNumberOfCreatures)
            .And(ShouldMapEachMonsterCorrectly);

        [Given]
        public void MonstersListContainsMultipleMonsters()
        {
            Monsters = new List<MonsterJson>
            {
                new MonsterJson
                {
                    Name = "Goblin",
                    ArmorClass = new List<JsonElement> { JsonDocument.Parse("15").RootElement },
                    HitPoints = new HitPointsJson { Average = 7, Formula = "2d6" },
                    Dexterity = 14,
                    Initiative = null
                },
                new MonsterJson
                {
                    Name = "Orc",
                    ArmorClass = new List<JsonElement> { JsonDocument.Parse("13").RootElement },
                    HitPoints = new HitPointsJson { Average = 15, Formula = "2d8+6" },
                    Dexterity = 12,
                    Initiative = null
                },
                new MonsterJson
                {
                    Name = "Hobgoblin Captain",
                    ArmorClass = new List<JsonElement> { JsonDocument.Parse("17").RootElement },
                    HitPoints = new HitPointsJson { Average = 39, Formula = "6d8+12" },
                    Dexterity = 14,
                    Initiative = new InitiativeJson { Proficiency = 2 }
                }
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnCorrectNumberOfCreatures()
        {
            Assert.That(Result, Has.Count.EqualTo(3));
        }

        [Then]
        public void ShouldMapEachMonsterCorrectly()
        {
            var goblin = Result[0];
            Assert.That(goblin.Name, Is.EqualTo("Goblin"));
            Assert.That(goblin.SystemName, Is.EqualTo("goblin"));
            Assert.That(goblin.ArmorClass, Is.EqualTo(15));
            Assert.That(goblin.HitPoints, Is.EqualTo(7));
            Assert.That(goblin.InitiativeModifier, Is.EqualTo(2)); // Dex 14 = +2

            var orc = Result[1];
            Assert.That(orc.Name, Is.EqualTo("Orc"));
            Assert.That(orc.SystemName, Is.EqualTo("orc"));
            Assert.That(orc.ArmorClass, Is.EqualTo(13));
            Assert.That(orc.HitPoints, Is.EqualTo(15));
            Assert.That(orc.InitiativeModifier, Is.EqualTo(1)); // Dex 12 = +1

            var hobgoblin = Result[2];
            Assert.That(hobgoblin.Name, Is.EqualTo("Hobgoblin Captain"));
            Assert.That(hobgoblin.SystemName, Is.EqualTo("hobgoblin-captain"));
            Assert.That(hobgoblin.ArmorClass, Is.EqualTo(17));
            Assert.That(hobgoblin.HitPoints, Is.EqualTo(39));
            Assert.That(hobgoblin.InitiativeModifier, Is.EqualTo(4)); // Dex 14 (+2) + proficiency (+2) = +4
        }
    }
}