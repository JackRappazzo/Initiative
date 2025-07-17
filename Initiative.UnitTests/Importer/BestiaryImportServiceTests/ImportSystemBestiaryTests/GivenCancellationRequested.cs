using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Encounters;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Initiative.UnitTests.Importer.BestiaryImportServiceTests.ImportSystemBestiaryTests
{
    public class GivenCancellationRequested : WhenTestingImportSystemBestiary
    {
        private CancellationTokenSource _cancellationTokenSource;
        private OperationCanceledException _cancellationException;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(FilePathIsSet)
            .And(BestiaryNameIsSet)
            .And(CancellationTokenIsSet)
            .And(CreatureImporterThrowsCancellationException)
            .When(ImportSystemBestiaryIsCalled)
            .Then(ShouldThrowOperationCanceledException)
            .And(ShouldNotCallBestiaryRepository);

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
        public void CancellationTokenIsSet()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Cancel();
            CancellationToken = _cancellationTokenSource.Token;
        }

        [Given]
        public void CreatureImporterThrowsCancellationException()
        {
            _cancellationException = new OperationCanceledException("Import operation was cancelled", CancellationToken);
            
            CreatureImporter.ImportFromFile(FilePath, CancellationToken)
                .Throws(_cancellationException);
        }

        [Then]
        public void ShouldThrowOperationCanceledException()
        {
            Assert.That(ThrownException, Is.Not.Null);
            Assert.That(ThrownException, Is.TypeOf<OperationCanceledException>());
            Assert.That(((OperationCanceledException)ThrownException).CancellationToken, Is.EqualTo(CancellationToken));
        }

        [Then]
        public void ShouldNotCallBestiaryRepository()
        {
            BestiaryRepository.DidNotReceive()
                .UpsertSystemBestiary(Arg.Any<string>(), Arg.Any<IEnumerable<Creature>>(), Arg.Any<CancellationToken>());
        }

        [TearDown]
        public void DisposeResources()
        {
            _cancellationTokenSource?.Dispose();
        }
    }
}