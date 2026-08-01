using Initiative.Persistence.Models.Condition;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Condition.ConditionServiceTests.GetConditionByNameTests
{
    public class GivenConditionDoesNotExist : WhenTestingGetConditionByName
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameIsSet)
            .And(RepositoryReturnsNull)
            .When(GetConditionByNameIsCalled)
            .Then(ShouldReturnNull);

        [Given]
        public void NameIsSet()
        {
            Name = "NonexistentCondition";
        }

        [Given]
        public void RepositoryReturnsNull()
        {
            ConditionRepository.GetConditionByName(Name, CancellationToken)
                .Returns((ConditionDocument?)null);
        }

        [Then]
        public void ShouldReturnNull()
        {
            Assert.That(Result, Is.Null);
        }
    }
}
