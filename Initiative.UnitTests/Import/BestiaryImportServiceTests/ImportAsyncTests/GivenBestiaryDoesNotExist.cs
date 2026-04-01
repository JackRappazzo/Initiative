using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Import.BestiaryImportServiceTests.ImportAsyncTests
{
    public class GivenBestiaryDoesNotExist : WhenTestingImportAsync
    {
        private string _newBestiaryId = "507f1f77bcf86cd799439011";

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BestiaryDoesNotExistInDatabase)
            .And(SourceProviderReturnsMonsterJson)
            .And(RepositoryReturnsNewId)
            .When(ImportAsyncIsCalled)
            .Then(ShouldCreateBestiary)
            .And(ShouldUpsertCreatures);

        [Given]
        public void BestiaryDoesNotExistInDatabase()
        {
            BestiaryRepository.GetBestiaryBySource("XMM", CancellationToken)
                .Returns((BestiaryDocument?)null);
        }

        [Given]
        public void SourceProviderReturnsMonsterJson()
        {
            SourceProvider.OpenSource("MonsterManual25.json").Returns(JsonStream(MinimalMonsterJson()));
            Parser.Parse(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>())
                .Returns([new BestiaryCreatureDocument
                {
                    BestiaryId = _newBestiaryId,
                    Source = "XMM",
                    Name = "Goblin",
                    RawData = new MongoDB.Bson.BsonDocument()
                }]);
        }

        [Given]
        public void RepositoryReturnsNewId()
        {
            BestiaryRepository.CreateBestiary(Arg.Any<BestiaryDocument>(), CancellationToken)
                .Returns(_newBestiaryId);
        }

        [Then]
        public void ShouldCreateBestiary()
        {
            BestiaryRepository.Received(1).CreateBestiary(
                Arg.Is<BestiaryDocument>(b =>
                    b.Name == "Monster Manual (2025)" &&
                    b.Source == "XMM" &&
                    b.OwnerId == null),
                CancellationToken);
        }

        [Then]
        public void ShouldUpsertCreatures()
        {
            BestiaryRepository.Received(1).UpsertCreatures(
                Arg.Is<IEnumerable<BestiaryCreatureDocument>>(c => c.Any()),
                CancellationToken);
        }
    }
}
