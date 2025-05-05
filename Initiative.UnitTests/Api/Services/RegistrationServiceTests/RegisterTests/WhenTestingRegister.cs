using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Api.Services.RegistrationServiceTests.RegisterTests
{
    public abstract class WhenTestingRegister : WhenTestingRegistrationService
    {
        protected bool IsSuccess;
        protected string ErrorMessage;

        protected string DisplayName, Email, Password;

        [When]
        public async Task RegisterIsCalled()
        {
            (IsSuccess, ErrorMessage) = await UserRegistrationService.RegisterUser(DisplayName, Email, Password, CancellationToken);
        }
    }
}
