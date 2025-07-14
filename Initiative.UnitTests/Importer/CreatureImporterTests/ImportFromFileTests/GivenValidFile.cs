using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromFileTests
{
    public class GivenValidFile : WhenTestingImportFromFile
    {
        private string _tempFilePath;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(ValidJsonFileIsCreated)
            .And(CreatureMapperReturnsMappedCreatures)
            .When(ImportFromFileAsyncIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnMappedCreatures)
            .And(CleanupTempFile);

        [Given]
        public void ValidJsonFileIsCreated()
        {
            _tempFilePath = Path.GetTempFileName();
            var jsonContent = """
            {
                "monster": [
                    {
                        "name": "Dragon",
                        "source": "MM",
                        "size": ["H"],
                        "type": "dragon",
                        "alignment": ["C", "E"],
                        "ac": [18],
                        "hp": {"average": 256},
                        "speed": {"walk": 40, "fly": 80},
                        "str": 27,
                        "dex": 14,
                        "con": 25,
                        "int": 16,
                        "wis": 13,
                        "cha": 21,
                        "cr": "17"
                    }
                ]
            }
            """;
            
            File.WriteAllText(_tempFilePath, jsonContent);
            FilePath = _tempFilePath;
        }

        [Given]
        public void CreatureMapperReturnsMappedCreatures()
        {
            var expectedCreatures = new List<Creature>
            {
                new Creature
                {
                    Name = "Dragon",
                    SystemName = "dragon",
                    ArmorClass = 18,
                    HitPoints = 256,
                    MaximumHitPoints = 256
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
            Assert.That(Result.First().Name, Is.EqualTo("Dragon"));
        }

        [Then]
        public void CleanupTempFile()
        {
            if (!string.IsNullOrEmpty(_tempFilePath) && File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }
    }
}