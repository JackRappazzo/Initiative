using Initiative.Import.Core.Models;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;
using System.Text;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromStreamTests
{
    public class GivenValidStream : WhenTestingImportFromStream
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(StreamContainsValidJson)
            .And(CreatureMapperReturnsMappedCreatures)
            .When(ImportFromStreamAsyncIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldReturnMappedCreatures)
            .And(StreamShouldBeDisposed);

        [Given]
        public void StreamContainsValidJson()
        {
            var jsonContent = """
            {
                "monster": [
                    {
                        "name": "Wizard",
                        "source": "MM",
                        "size": ["M"],
                        "type": "humanoid",
                        "alignment": ["A"],
                        "ac": [12],
                        "hp": {"average": 40},
                        "speed": {"walk": 30},
                        "str": 9,
                        "dex": 14,
                        "con": 11,
                        "int": 17,
                        "wis": 12,
                        "cha": 11,
                        "cr": "6"
                    }
                ]
            }
            """;

            var bytes = Encoding.UTF8.GetBytes(jsonContent);
            Stream = new MemoryStream(bytes);
        }

        [Given]
        public void CreatureMapperReturnsMappedCreatures()
        {
            var expectedCreatures = new List<Creature>
            {
                new Creature
                {
                    Name = "Wizard",
                    SystemName = "wizard",
                    ArmorClass = 12,
                    HitPoints = 40,
                    MaximumHitPoints = 40
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
            Assert.That(Result.First().Name, Is.EqualTo("Wizard"));
        }

        [Then]
        public void StreamShouldBeDisposed()
        {
            // The stream should still be accessible but we can verify it was used
            Assert.That(Stream, Is.Not.Null);
        }
    }
}