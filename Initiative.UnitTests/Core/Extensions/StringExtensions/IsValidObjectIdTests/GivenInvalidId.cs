using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;

namespace Initiative.UnitTests.Core.Extensions.StringExtensions.IsValidObjectIdTests
{
    public class GivenInvalidId : WhenTestingIsObjectId
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(TestStringIsInvalidId)
            .When(IsValidObjectIdIsCalled)
            .Then(ShouldReturnFalse);

        [Given]
        public void TestStringIsInvalidId()
        {
            TestString = "invalid-object-id";
        }

        [Then]
        public void ShouldReturnFalse()
        {
            Assert.That(Result, Is.False);
        }
    }
}
