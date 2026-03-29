using Initiative.Persistence.Models.Bestiary;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Import.FivEToolsParserTests.ParseTests
{
    public abstract class WhenTestingParse : WhenTestingFivEToolsParser
    {
        protected const string BestiaryId = "507f1f77bcf86cd799439011";
        protected const string Source = "TST";

        protected Stream InputStream;
        protected List<BestiaryCreatureDocument> Result;

        [When(DoNotRethrowExceptions: true)]
        public void ParseIsCalled()
        {
            Result = Parser.Parse(BestiaryId, Source, InputStream);
        }

        protected static Stream JsonStream(string json)
            => new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
    }
}
