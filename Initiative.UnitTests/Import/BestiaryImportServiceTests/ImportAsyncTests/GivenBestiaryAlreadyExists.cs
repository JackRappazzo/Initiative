using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Import.BestiaryImportServiceTests.ImportAsyncTests
{
    public class GivenBestiaryAlreadyExists : WhenTestingImportAsync
    {
        private string _existingBestiaryId = "507f1f77bcf86cd799439011";

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryAlreadyExistsInDatabase)
            .And(SourceProviderReturnsMonsterJson)
            .When(ImportAsyncIsCalled)
            .Then(ShouldNotCreateBestiary)
            .And(ShouldStillUpsertCreatures);

        [Given]
        public void BestiaryAlreadyExistsInDatabase()
        {
            BestiaryRepository.GetBestiaryBySource("XMM", CancellationToken)
                .Returns(new BestiaryDocument
                {
                    Id = _existingBestiaryId,
                    Name = "Monster Manual (2025)",
                    Source = "XMM",
                    IsSystem = true
                });
        }

        [Given]
        public void SourceProviderReturnsMonsterJson()
        {
            SourceProvider.OpenMonsterManual25().Returns(JsonStream(MinimalMonsterJson()));
            Parser.Parse(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>())
                .Returns([new BestiaryCreatureDocument
                {
                    BestiaryId = _existingBestiaryId,
                    Source = "XMM",
                    Name = "Goblin",
                    RawData = new MongoDB.Bson.BsonDocument()
                }]);
        }

        [Then]
        public void ShouldNotCreateBestiary()
        {
            BestiaryRepository.DidNotReceive().CreateBestiary(
                Arg.Any<BestiaryDocument>(),
                Arg.Any<CancellationToken>());
        }

        [Then]
        public void ShouldStillUpsertCreatures()
        {
            BestiaryRepository.Received(1).UpsertCreatures(
                Arg.Is<IEnumerable<BestiaryCreatureDocument>>(c => c.Any()),
                CancellationToken);
        }
    }
}
