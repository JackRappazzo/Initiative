using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.BestiaryRepositoryTests.GetCreatureByIdTests
{
    public class GivenCreatureDoesNotExist : WhenTestingGetCreatureById
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(CreatureIdIsUnknown)
            .When(GetCreatureByIdIsCalled)
            .Then(ShouldReturnNull);

        [Given]
        public void CreatureIdIsUnknown()
        {
            CreatureId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        }

        [Then]
        public void ShouldReturnNull()
        {
            Assert.That(Result, Is.Null);
        }
    }
}
