using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromJsonTests
{
    public class GivenValidJsonWithSingleMonster : WhenTestingImportFromJson
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(JsonContentIsSetWithSingleMonster)
            .And(CreatureMapperReturnsMappedCreatures)
            .When(ImportFromJsonIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnMappedCreatures);

        [Given]
        public void JsonContentIsSetWithSingleMonster()
        {
            JsonContent = """
            {
                "monster": [
                    {
                        "name": "Goblin",
                        "source": "MM",
                        "size": ["S"],
                        "type": "humanoid",
                        "alignment": ["C", "E"],
                        "ac": [15],
                        "hp": {"average": 7},
                        "speed": {"walk": 30},
                        "str": 8,
                        "dex": 14,
                        "con": 10,
                        "int": 10,
                        "wis": 8,
                        "cha": 8,
                        "cr": "1/4"
                    }
                ]
            }
            """;
        }

        [Given]
        public void CreatureMapperReturnsMappedCreatures()
        {
            var expectedCreatures = new List<Creature>
            {
                new Creature
                {
                    Name = "Goblin",
                    SystemName = "goblin",
                    ArmorClass = 15,
                    HitPoints = 7,
                    MaximumHitPoints = 7,
                    InitiativeModifier = 2,
                    Initiative = 0,
                    IsConcentrating = false,
                    IsPlayer = false
                }
            };

            CreatureMapper.MapToCreatures(Arg.Any<IEnumerable<MonsterJson>>())
                .Returns(expectedCreatures);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldReturnMappedCreatures()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Has.Count.EqualTo(1));
            Assert.That(Result.First().Name, Is.EqualTo("Goblin"));
        }
    }
}