using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using Initiative.Api.Core.Services.Authentication;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Core.Authentication.JwtServiceTests.GenerateRefreshTokenTests
{
    public abstract class WhenTestingGenerateRefreshToken : WhenTestingJwtService
    {
        protected JwtRefreshToken Result;

        protected ApplicationIdentity User;
        protected string Token;
        protected DateTime Expiration;

        [When]
        public async Task GenerateRefreshTokenIsCalled()
        {
            Result = await JwtService.GenerateAndStoreRefreshToken(User, Expiration, CancellationToken);
        }
    }
}
