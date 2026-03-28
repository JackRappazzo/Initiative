using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.GetBestiaryBySourceTests
{
    public class GivenBestiaryDoesNotExist : WhenTestingGetBestiaryBySource
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(SourceIsSetToUnknownValue)
            .When(GetBestiaryBySourceIsCalled)
            .Then(ShouldReturnNull);

        [Given]
        public void SourceIsSetToUnknownValue()
        {
            Source = "DOES_NOT_EXIST";
        }

        [Then]
        public void ShouldReturnNull()
        {
            Assert.That(Result, Is.Null);
        }
    }
}
