using System.Text.Json;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.ArmorClassExtractorTests
{
    public abstract class WhenTestingExtractArmorClass : WhenTestingArmorClassExtractor
    {
        protected List<JsonElement>? ArmorClassList;
        protected int Result;

        [When(DoNotRethrowExceptions: true)]
        public void ExtractArmorClassIsCalled()
        {
            Result = ArmorClassExtractor.ExtractArmorClass(ArmorClassList);
        }
    }
}