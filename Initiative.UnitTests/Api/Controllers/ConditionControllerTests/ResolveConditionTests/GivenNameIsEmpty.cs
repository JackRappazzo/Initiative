using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Initiative.UnitTests.Api.Controllers.ConditionControllerTests.ResolveConditionTests
{
    public class GivenNameIsEmpty : WhenTestingResolveCondition
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameIsEmpty)
            .When(ResolveConditionIsCalled)
            .Then(ShouldReturnBadRequest);

        [Given]
        public void NameIsEmpty()
        {
            QueryName = "";
        }

        [Then]
        public void ShouldReturnBadRequest()
        {
            Assert.That(Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
