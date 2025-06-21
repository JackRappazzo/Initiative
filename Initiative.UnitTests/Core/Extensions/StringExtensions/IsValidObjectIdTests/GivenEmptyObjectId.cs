using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.UnitTests.Core.Extensions.StringExtensions.IsValidObjectIdTests
{
    public class GivenEmptyObjectId : WhenTestingIsObjectId
    {

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(TestStringIsEmptyObjectId)
            .When(IsValidObjectIdIsCalled)
            .Then(ShouldReturnFalse);

        [Given]
        public void TestStringIsEmptyObjectId()
        {
            TestString = ObjectId.Empty.ToString();
        }

        [Then]
        public void ShouldReturnFalse()
        {
            Assert.That(Result, Is.False, "Empty ObjectId should return false");
        }
    }
}
