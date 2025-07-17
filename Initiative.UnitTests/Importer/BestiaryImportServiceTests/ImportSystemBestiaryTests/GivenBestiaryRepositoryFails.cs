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
using NSubstitute.ExceptionExtensions;

namespace Initiative.UnitTests.Importer.BestiaryImportServiceTests.ImportSystemBestiaryTests
{
    public class GivenBestiaryRepositoryFails : WhenTestingImportSystemBestiary
    {
        private List<Creature> _importedCreatures;
        private InvalidOperationException _repositoryException;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(FilePathIsSet)
            .And(BestiaryNameIsSet)
            .And(CreatureImporterReturnsCreatures)
            .And(BestiaryRepositoryThrowsException)
            .When(ImportSystemBestiaryIsCalled)
            .Then(ShouldPropagateRepositoryException)
            .And(ShouldCallCreatureImporterFirst);

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
                    Name = "Test Dragon",
                    SystemName = "test-dragon",
                    ArmorClass = 18,
                    HitPoints = 200,
                    MaximumHitPoints = 200,
                    IsPlayer = false,
                    IsConcentrating = false
                }
            };

            CreatureImporter.ImportFromFile(FilePath, CancellationToken)
                .Returns(_importedCreatures);
        }

        [Given]
        public void BestiaryRepositoryThrowsException()
        {
            _repositoryException = new InvalidOperationException("Database connection failed");
            
            BestiaryRepository.UpsertSystemBestiary(BestiaryName, _importedCreatures, CancellationToken)
                .Throws(_repositoryException);
        }

        [Then]
        public void ShouldPropagateRepositoryException()
        {
            Assert.That(ThrownException, Is.Not.Null);
            Assert.That(ThrownException, Is.SameAs(_repositoryException));
        }

        [Then]
        public void ShouldCallCreatureImporterFirst()
        {
            CreatureImporter.Received(1)
                .ImportFromFile(FilePath, CancellationToken);
        }
    }
}