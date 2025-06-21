using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using Initiative.Api.Core.Services.Authentication;
using Initiative.Api.Core.Services.Users;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace Initiative.UnitTests.Api.Services.LoginServiceTests
{
    public abstract class WhenTestingLoginService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest] protected UserLoginService UserLoginService;
        [Dependency] protected IJwtService JwtService;
        [Dependency] protected IUserManager<ApplicationIdentity> UserManager;

        protected CancellationToken CancellationToken = default(CancellationToken);


    }
}
