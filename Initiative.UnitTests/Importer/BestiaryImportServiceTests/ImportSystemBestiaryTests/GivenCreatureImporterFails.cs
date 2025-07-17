using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Initiative.UnitTests.Importer.BestiaryImportServiceTests.ImportSystemBestiaryTests
{
    public class GivenCreatureImporterFails : WhenTestingImportSystemBestiary
    {
        private FileNotFoundException _importerException;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(FilePathIsSet)
            .And(BestiaryNameIsSet)
            .And(CreatureImporterThrowsException)
            .When(ImportSystemBestiaryIsCalled)
            .Then(ShouldPropagateImporterException)
            .And(ShouldNotCallBestiaryRepository);

        [Given]
        public void FilePathIsSet()
        {
            FilePath = "NonExistent/monsters.json";
        }

        [Given]
        public void BestiaryNameIsSet()
        {
            BestiaryName = "Test Monster Manual";
        }

        [Given]
        public void CreatureImporterThrowsException()
        {
            _importerException = new FileNotFoundException("The specified file was not found", FilePath);
            
            CreatureImporter.ImportFromFile(FilePath, CancellationToken)
                .Throws(_importerException);
        }

        [Then]
        public void ShouldPropagateImporterException()
        {
            Assert.That(ThrownException, Is.Not.Null);
            Assert.That(ThrownException, Is.SameAs(_importerException));
        }

        [Then]
        public void ShouldNotCallBestiaryRepository()
        {
            BestiaryRepository.DidNotReceive()
                .UpsertSystemBestiary(Arg.Any<string>(), Arg.Any<IEnumerable<Persistence.Models.Encounters.Creature>>(), Arg.Any<CancellationToken>());
        }
    }
}