using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Import.FivEToolsParserTests.ParseTests
{
    public class GivenMissingMonsterArray : WhenTestingParse
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(InputHasNoMonsterArray)
            .When(ParseIsCalled)
            .Then(ShouldThrowInvalidOperationException);

        [Given]
        public void InputHasNoMonsterArray()
        {
            InputStream = JsonStream("""{ "notMonsters": [] }""");
        }

        [Then]
        public void ShouldThrowInvalidOperationException()
        {
            Assert.That(ThrownException, Is.InstanceOf<InvalidOperationException>());
        }
    }
}
