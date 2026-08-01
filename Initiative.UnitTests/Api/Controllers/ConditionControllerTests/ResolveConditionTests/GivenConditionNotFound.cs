using Initiative.Persistence.Models.Condition;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Initiative.UnitTests.Api.Controllers.ConditionControllerTests.ResolveConditionTests
{
    public class GivenConditionNotFound : WhenTestingResolveCondition
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameIsSet)
            .And(ServiceReturnsNull)
            .When(ResolveConditionIsCalled)
            .Then(ShouldReturnNotFound);

        [Given]
        public void NameIsSet()
        {
            QueryName = "NonexistentCondition";
        }

        [Given]
        public void ServiceReturnsNull()
        {
            ConditionService.GetConditionByName(QueryName, CancellationToken)
                .Returns((ConditionDocument?)null);
        }

        [Then]
        public void ShouldReturnNotFound()
        {
            Assert.That(Result, Is.InstanceOf<NotFoundResult>());
        }
    }
}
