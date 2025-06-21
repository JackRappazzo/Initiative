using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Extensions;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Core.Extensions.StringExtensions.IsValidObjectIdTests
{
    public abstract class WhenTestingIsObjectId : ComposableTestingTheBehaviourOf
    {
        protected bool Result;
        protected string TestString;

        [When]
        public void IsValidObjectIdIsCalled()
        {
            Result = TestString.IsValidObjectId();
        }
    }
}
