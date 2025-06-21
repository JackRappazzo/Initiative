using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Core.Authentication.JwtServiceTests.GenerateTokenTests
{
    public abstract class WhenTestingCreate : WhenTestingJwtService
    {
        protected ApplicationIdentity User;

        protected string Result;

        [When]
        public void CreateIsCalled()
        {
            Result = JwtService.GenerateToken(User);
        }
    }
}
