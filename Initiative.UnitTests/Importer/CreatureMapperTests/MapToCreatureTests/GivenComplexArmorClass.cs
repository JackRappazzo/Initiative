using Initiative.Import.Core.Models;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using System.Text.Json;

namespace Initiative.UnitTests.Importer.CreatureMapperTests.MapToCreatureTests
{
    public class GivenComplexArmorClass : WhenTestingMapToCreature
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(MonsterJsonHasObjectArmorClass)
            .When(MapToCreatureIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldExtractArmorClassFromObject);

        [Given]
        public void MonsterJsonHasObjectArmorClass()
        {
            var acJson = """{"ac": 18, "from": ["plate armor"]}""";
            
            MonsterJson = new MonsterJson
            {
                Name = "Paladin",
                ArmorClass = new List<JsonElement> { JsonDocument.Parse(acJson).RootElement },
                HitPoints = new HitPointsJson { Average = 58, Formula = "9d8+18" },
                Dexterity = 9, // Should give -1 modifier
                Initiative = null
            };
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldExtractArmorClassFromObject()
        {
            Assert.That(Result.ArmorClass, Is.EqualTo(18));
        }
    }
}