using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using Initiative.Api.Core.Services.Users;
using Initiative.Api.Core.Utilities;
using Initiative.Persistence.Repositories;
using Initiative.UnitTests.Api.Helpers;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace Initiative.UnitTests.Api.Services.UserServiceTests
{
    public abstract class WhenTestingUserService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest]
        protected UserService UserService;

        [Dependency]
        protected IUserManager<ApplicationIdentity> UserManager;

        [Dependency]
        protected IInitiativeUserRepository UserRepository;

        [Dependency]
        protected IBase62CodeGenerator Base62CodeGenerator = Substitute.For<IBase62CodeGenerator>();

        protected CancellationToken CancellationToken = default;


    }
}
