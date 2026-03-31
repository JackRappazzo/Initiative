using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Bestiary.BestiaryTriesServiceTests.GetCreatureByIdTests
{
    public class GivenCreatureDoesNotExist : WhenTestingGetCreatureById
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(CreatureIdIsSet)
            .And(RepositoryReturnsNull)
            .When(GetCreatureByIdIsCalled)
            .Then(ShouldReturnNull);

        [Given]
        public void CreatureIdIsSet()
        {
            CreatureId = NewId();
        }

        [Given]
        public void RepositoryReturnsNull()
        {
            BestiaryRepository.GetCreatureById(CreatureId, CancellationToken)
                .Returns((Initiative.Persistence.Models.Bestiary.BestiaryCreatureDocument?)null);
        }

        [Then]
        public void ShouldReturnNull()
        {
            Assert.That(Result, Is.Null);
        }
    }
}
