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

        protected bool IsSuccess;
        protected string ErrorMessage;
        protected string Jwt;


        protected string Email;
        protected string Password;

        [When]
        public async Task LoginIsCalled()
        {
            (IsSuccess, ErrorMessage, Jwt) = await UserLoginService.LoginAndFetchToken(Email, Password, CancellationToken);
        }
    }
}
