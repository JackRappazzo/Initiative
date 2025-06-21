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
    public class GivenObjectId : WhenTestingIsObjectId
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(TestStringIsObjectId)
            .When(IsValidObjectIdIsCalled)
            .Then(ShouldReturnTrue);

        [Given]
        public void TestStringIsObjectId()
        {
            TestString = "684b2c436e63bd88dacab4f7";
        }

        [Then]
        public void ShouldReturnTrue()
        {
            Assert.That(Result, Is.True);
        }
        
    }
}
