using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using Initiative.Api.Services;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Api.Services.LoginServiceTests.LoginTests
{
    public abstract class WhenTestingLogin : WhenTestingLoginService
    {

        protected LoginResult Result;


        protected string Email;
        protected string Password;

        [When]
        public async Task LoginIsCalled()
        {
            Result = await UserLoginService.LoginAndFetchTokens(Email, Password, CancellationToken);
        }
    }
}
