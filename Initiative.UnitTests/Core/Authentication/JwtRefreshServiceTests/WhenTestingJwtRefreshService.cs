using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Authentication;
using Initiative.Api.Core.Identity;
using Initiative.Persistence.Repositories;
using Initiative.UnitTests.Api.Helpers;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace Initiative.UnitTests.Core.Authentication.JwtRefreshServiceTests
{
    public abstract class WhenTestingJwtRefreshService : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest] protected JwtRefreshService JwtRefreshService;
        [Dependency] protected IJwtRefreshTokenRepository JwtRefreshTokenRepository;
        [Dependency] protected IJwtService JwtService;
        [Dependency] protected UserManager<InitiativeUser> UserManager;

        protected CancellationToken CancellationToken = default(CancellationToken);

        protected override void CreateManualDependencies()
        {
            UserManager = Substitute.For<MockUserManager<InitiativeUser>>();
            base.CreateManualDependencies();
        }
    }
}
