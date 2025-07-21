using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Importer.ExtractorTests.SystemNameGeneratorTests
{
    public abstract class WhenTestingGenerateSystemName : WhenTestingSystemNameGenerator
    {
        protected string Name;
        protected string Result;

        [When(DoNotRethrowExceptions: true)]
        public void GenerateSystemNameIsCalled()
        {
            Result = SystemNameGenerator.GenerateSystemName(Name);
        }
    }
}