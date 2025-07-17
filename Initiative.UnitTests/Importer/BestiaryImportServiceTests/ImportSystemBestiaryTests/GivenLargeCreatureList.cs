using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Importer.BestiaryImportServiceTests.ImportSystemBestiaryTests
{
    public class GivenLargeCreatureList : WhenTestingImportSystemBestiary
    {
        private List<Creature> _largeCreatureList;
        private string _expectedBestiaryId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(FilePathIsSet)
            .And(BestiaryNameIsSet)
            .And(CreatureImporterReturnsLargeList)
            .And(BestiaryRepositoryReturnsSuccessfully)
            .When(ImportSystemBestiaryIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldCallBestiaryRepositoryWithAllCreatures);

        [Given]
        public void FilePathIsSet()
        {
            FilePath = "TestData/large-monster-manual.json";
        }

        [Given]
        public void BestiaryNameIsSet()
        {
            BestiaryName = "Large Monster Manual";
        }

        [Given]
        public void CreatureImporterReturnsLargeList()
        {
            _largeCreatureList = new List<Creature>();
            
            // Create 500 test creatures to simulate a large import
            for (int i = 1; i <= 500; i++)
            {
                _largeCreatureList.Add(new Creature
                {
                    Name = $"Monster {i}",
                    SystemName = $"monster-{i}",
                    ArmorClass = 10 + (i % 20),
                    HitPoints = 5 + (i % 100),
                    MaximumHitPoints = 5 + (i % 100),
                    InitiativeModifier = i % 5,
                    WalkSpeed = 30,
                    IsPlayer = false,
                    IsConcentrating = false
                });
            }

            CreatureImporter.ImportFromFile(FilePath, CancellationToken)
                .Returns(_largeCreatureList);
        }

        [Given]
        public void BestiaryRepositoryReturnsSuccessfully()
        {
            _expectedBestiaryId = Guid.NewGuid().ToString();
            BestiaryRepository.UpsertSystemBestiary(BestiaryName, _largeCreatureList, CancellationToken)
                .Returns(_expectedBestiaryId);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldCallBestiaryRepositoryWithAllCreatures()
        {
            BestiaryRepository.Received(1)
                .UpsertSystemBestiary(BestiaryName, 
                    Arg.Is<IEnumerable<Creature>>(creatures => creatures.Count() == 500), 
                    CancellationToken);
        }
    }
}