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
    public class GivenEmptyCreatureList : WhenTestingImportSystemBestiary
    {
        private List<Creature> _emptyCreatureList;
        private string _expectedBestiaryId;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(FilePathIsSet)
            .And(BestiaryNameIsSet)
            .And(CreatureImporterReturnsEmptyList)
            .And(BestiaryRepositoryReturnsSuccessfully)
            .When(ImportSystemBestiaryIsCalled)
            .Then(ShouldNotThrowException)
            .And(ShouldCallBestiaryRepositoryWithEmptyList);

        [Given]
        public void FilePathIsSet()
        {
            FilePath = "TestData/empty-monsters.json";
        }

        [Given]
        public void BestiaryNameIsSet()
        {
            BestiaryName = "Empty Monster Manual";
        }

        [Given]
        public void CreatureImporterReturnsEmptyList()
        {
            _emptyCreatureList = new List<Creature>();

            CreatureImporter.ImportFromFile(FilePath, CancellationToken)
                .Returns(_emptyCreatureList);
        }

        [Given]
        public void BestiaryRepositoryReturnsSuccessfully()
        {
            _expectedBestiaryId = Guid.NewGuid().ToString();
            BestiaryRepository.UpsertSystemBestiary(BestiaryName, _emptyCreatureList, CancellationToken)
                .Returns(_expectedBestiaryId);
        }

        [Then]
        public void ShouldNotThrowException()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void ShouldCallBestiaryRepositoryWithEmptyList()
        {
            BestiaryRepository.Received(1)
                .UpsertSystemBestiary(BestiaryName, Arg.Is<IEnumerable<Creature>>(creatures => !creatures.Any()), CancellationToken);
        }
    }
}