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
    public class GivenHappyPath : WhenTestingImportSystemBestiary
    {
        private List<Creature> _importedCreatures;
        private string _expectedBestiaryId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(FilePathIsSet)
            .And(BestiaryNameIsSet)
            .And(CreatureImporterReturnsCreatures)
            .And(BestiaryRepositoryReturnsSuccessfully)
            .When(ImportSystemBestiaryIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldCallCreatureImporterWithCorrectParameters)
            .And(ShouldCallBestiaryRepositoryWithCorrectParameters);

        [Given]
        public void FilePathIsSet()
        {
            FilePath = "TestData/monsters.json";
        }

        [Given]
        public void BestiaryNameIsSet()
        {
            BestiaryName = "Test Monster Manual";
        }

        [Given]
        public void CreatureImporterReturnsCreatures()
        {
            _importedCreatures = new List<Creature>
            {
                new Creature
                {
                    Name = "Adult Red Dragon",
                    SystemName = "adult-red-dragon",
                    ArmorClass = 19,
                    HitPoints = 256,
                    MaximumHitPoints = 256,
                    InitiativeModifier = 0,
                    WalkSpeed = 40,
                    FlySpeed = 80,
                    IsPlayer = false,
                    IsConcentrating = false
                },
                new Creature
                {
                    Name = "Goblin",
                    SystemName = "goblin", 
                    ArmorClass = 15,
                    HitPoints = 7,
                    MaximumHitPoints = 7,
                    InitiativeModifier = 2,
                    WalkSpeed = 30,
                    IsPlayer = false,
                    IsConcentrating = false
                }
            };

            CreatureImporter.ImportFromFile(FilePath, CancellationToken)
                .Returns(_importedCreatures);
        }

        [Given]
        public void BestiaryRepositoryReturnsSuccessfully()
        {
            _expectedBestiaryId = Guid.NewGuid().ToString();
            BestiaryRepository.UpsertSystemBestiary(BestiaryName, _importedCreatures, CancellationToken)
                .Returns(_expectedBestiaryId);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldCallCreatureImporterWithCorrectParameters()
        {
            CreatureImporter.Received(1)
                .ImportFromFile(FilePath, CancellationToken);
        }

        [Then]
        public void ShouldCallBestiaryRepositoryWithCorrectParameters()
        {
            BestiaryRepository.Received(1)
                .UpsertSystemBestiary(BestiaryName, _importedCreatures, CancellationToken);
        }
    }
}