using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromJsonTests
{
    public class GivenValidJsonWithMultipleMonsters : WhenTestingImportFromJson
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(JsonContentIsSetWithMultipleMonsters)
            .And(CreatureMapperReturnsMappedCreatures)
            .When(ImportFromJsonIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnAllMappedCreatures);

        [Given]
        public void JsonContentIsSetWithMultipleMonsters()
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
                    },
                    {
                        "name": "Orc",
                        "source": "MM",
                        "size": ["M"],
                        "type": "humanoid",
                        "alignment": ["C", "E"],
                        "ac": [13],
                        "hp": {"average": 15},
                        "speed": {"walk": 30},
                        "str": 16,
                        "dex": 12,
                        "con": 16,
                        "int": 7,
                        "wis": 11,
                        "cha": 10,
                        "cr": "1/2"
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
                new Creature { Name = "Goblin", SystemName = "goblin" },
                new Creature { Name = "Orc", SystemName = "orc" }
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
        public void ShouldReturnAllMappedCreatures()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Has.Count.EqualTo(2));
            Assert.That(Result.Any(c => c.Name == "Goblin"), Is.True);
            Assert.That(Result.Any(c => c.Name == "Orc"), Is.True);
        }
    }
}