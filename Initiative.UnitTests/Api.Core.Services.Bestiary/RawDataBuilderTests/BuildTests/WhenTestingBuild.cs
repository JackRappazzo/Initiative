using Initiative.Api.Core.Services.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using MongoDB.Bson;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.RawDataBuilderTests.BuildTests
{
    public abstract class WhenTestingBuild : WhenTestingCustomCreatureRawDataBuilder
    {
        protected CustomCreatureData Data;
        protected BsonDocument Result;

        [When]
        public void BuildIsCalled()
        {
            Result = Builder.Build(Data);
        }
    }
}
