using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Initiative.UnitTests.Api.Controllers.ConditionControllerTests.ResolveConditionTests
{
    public class GivenNameIsWhitespace : WhenTestingResolveCondition
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(NameIsWhitespace)
            .When(ResolveConditionIsCalled)
            .Then(ShouldReturnBadRequest);

        [Given]
        public void NameIsWhitespace()
        {
            QueryName = "   ";
        }

        [Then]
        public void ShouldReturnBadRequest()
        {
            Assert.That(Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
