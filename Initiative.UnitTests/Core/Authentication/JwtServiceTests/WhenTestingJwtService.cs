using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Authentication;
using Initiative.Persistence.Repositories;
using LeapingGorilla.Testing.Core;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Initiative.UnitTests.Core.Authentication.JwtServiceTests
{
    public abstract class WhenTestingJwtService : ComposableTestingTheBehaviourOf
    {

        [ItemUnderTest] protected JwtService JwtService;

        [Dependency] protected ICredentialsFactory CredentialsFactory;
        [Dependency] protected IOptions<JwtSettings> JwtSettingsContainer;
        [Dependency] protected IJwtRefreshTokenRepository JwtRefreshTokenRepository;

        protected CancellationToken CancellationToken = default;

        protected JwtSettings JwtSettings;

    }
}
