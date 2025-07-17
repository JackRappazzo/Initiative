using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Importer.BestiaryImportServiceTests.ImportSystemBestiaryTests
{
    public class GivenWhitespaceBestiaryName : WhenTestingImportSystemBestiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(FilePathIsSet)
            .And(BestiaryNameIsWhitespace)
            .When(ImportSystemBestiaryIsCalled)
            .Then(ShouldThrowArgumentException)
            .And(ShouldNotCallCreatureImporter)
            .And(ShouldNotCallBestiaryRepository);

        [Given]
        public void FilePathIsSet()
        {
            FilePath = "TestData/monsters.json";
        }

        [Given]
        public void BestiaryNameIsWhitespace()
        {
            BestiaryName = "   \t\n   ";
        }

        [Then]
        public void ShouldThrowArgumentException()
        {
            Assert.That(ThrownException, Is.Not.Null);
            Assert.That(ThrownException, Is.TypeOf<ArgumentException>());
            Assert.That(ThrownException.Message, Does.Contain("Bestiary name cannot be null or empty"));
            Assert.That(((ArgumentException)ThrownException).ParamName, Is.EqualTo("bestiaryName"));
        }

        [Then]
        public void ShouldNotCallCreatureImporter()
        {
            CreatureImporter.DidNotReceive()
                .ImportFromFile(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Then]
        public void ShouldNotCallBestiaryRepository()
        {
            BestiaryRepository.DidNotReceive()
                .UpsertSystemBestiary(Arg.Any<string>(), Arg.Any<IEnumerable<Persistence.Models.Encounters.Creature>>(), Arg.Any<CancellationToken>());
        }
    }
}