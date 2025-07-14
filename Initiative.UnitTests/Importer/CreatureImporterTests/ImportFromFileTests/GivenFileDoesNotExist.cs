using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.CreatureImporterTests.ImportFromFileTests
{
    public class GivenFileDoesNotExist : WhenTestingImportFromFile
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(FilePathIsSetToNonExistentFile)
            .When(ImportFromFileAsyncIsCalled)
            .Then(ShouldThrowFileNotFoundException);

        [Given]
        public void FilePathIsSetToNonExistentFile()
        {
            FilePath = "nonexistent-file.json";
        }

        [Then]
        public void ShouldThrowFileNotFoundException()
        {
            Assert.That(ThrownException, Is.Not.Null);
            Assert.That(ThrownException, Is.InstanceOf<FileNotFoundException>());
            Assert.That(ThrownException.Message, Does.Contain($"File not found: {FilePath}"));
        }
    }
}