using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Api.Services.UserServiceTests.RegisterTests
{
    public abstract class WhenTestingRegister : WhenTestingUserService
    {
        protected bool IsSuccess;
        protected string ErrorMessage;

        protected string DisplayName, Email, Password;

        [When]
        public async Task RegisterIsCalled()
        {
            (IsSuccess, ErrorMessage) = await UserService.RegisterUser(DisplayName, Email, Password, CancellationToken);
        }
    }
}
