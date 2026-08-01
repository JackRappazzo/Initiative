using Initiative.Persistence.Models.Condition;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Api.Core.Services.Condition.ConditionServiceTests.GetConditionByNameTests
{
    public class GivenConditionExists : WhenTestingGetConditionByName
    {
        private ConditionDocument _condition = null!;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameIsSet)
            .And(RepositoryReturnsCondition)
            .When(GetConditionByNameIsCalled)
            .Then(ShouldReturnCondition)
            .And(ShouldForwardNameToRepository);

        [Given]
        public void NameIsSet()
        {
            Name = "Grappled";
        }

        [Given]
        public void RepositoryReturnsCondition()
        {
            _condition = new ConditionDocument
            {
                Source = "XPHB",
                Name = "Grappled",
                Type = "condition",
                RawData = new BsonDocument()
            };

            ConditionRepository.GetConditionByName(Name, CancellationToken)
                .Returns(_condition);
        }

        [Then]
        public void ShouldReturnCondition()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result!.Name, Is.EqualTo("Grappled"));
            Assert.That(Result.Source, Is.EqualTo("XPHB"));
            Assert.That(Result.Type, Is.EqualTo("condition"));
        }

        [Then]
        public void ShouldForwardNameToRepository()
        {
            ConditionRepository.Received(1).GetConditionByName(Name, CancellationToken);
        }
    }
}
