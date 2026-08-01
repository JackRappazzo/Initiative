using Initiative.Persistence.Models.Condition;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Api.Controllers.ConditionControllerTests.ResolveConditionTests
{
    public class GivenConditionFound : WhenTestingResolveCondition
    {
        private ConditionDocument _condition = null!;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameIsSet)
            .And(ServiceReturnsCondition)
            .When(ResolveConditionIsCalled)
            .Then(ShouldReturnOkWithJson)
            .And(ShouldContainConditionNameInResponse)
            .And(ShouldForwardNameToService);

        [Given]
        public void NameIsSet()
        {
            QueryName = "Grappled";
        }

        [Given]
        public void ServiceReturnsCondition()
        {
            _condition = new ConditionDocument
            {
                Source = "XPHB",
                Name = "Grappled",
                Type = "condition",
                RawData = new BsonDocument
                {
                    { "name", "Grappled" },
                    { "source", "XPHB" },
                    { "entries", new BsonArray { "Speed becomes 0." } }
                }
            };

            ConditionService.GetConditionByName(QueryName, CancellationToken)
                .Returns(_condition);
        }

        [Then]
        public void ShouldReturnOkWithJson()
        {
            Assert.That(Result, Is.InstanceOf<ContentResult>());
            var contentResult = (ContentResult)Result;
            Assert.That(contentResult.ContentType, Is.EqualTo("application/json"));
        }

        [Then]
        public void ShouldContainConditionNameInResponse()
        {
            var contentResult = (ContentResult)Result;
            Assert.That(contentResult.Content, Does.Contain("Grappled"));
        }

        [Then]
        public void ShouldForwardNameToService()
        {
            ConditionService.Received(1).GetConditionByName(QueryName, CancellationToken);
        }
    }
}
