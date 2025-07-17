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
    public class GivenOperationOrder : WhenTestingImportSystemBestiary
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
            .And(ShouldCallOperationsInCorrectOrder);

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
                    Name = "Test Creature",
                    SystemName = "test-creature",
                    ArmorClass = 15,
                    HitPoints = 30,
                    MaximumHitPoints = 30,
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
        public void ShouldCallOperationsInCorrectOrder()
        {
            Received.InOrder(() =>
            {
                CreatureImporter.ImportFromFile(FilePath, CancellationToken);
                BestiaryRepository.UpsertSystemBestiary(BestiaryName, _importedCreatures, CancellationToken);
            });
        }
    }
}