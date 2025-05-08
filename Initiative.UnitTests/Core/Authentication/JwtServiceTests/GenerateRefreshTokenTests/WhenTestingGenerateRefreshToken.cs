using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Authentication;
using Initiative.Api.Core.Identity;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.UnitTests.Core.Authentication.JwtServiceTests.GenerateRefreshTokenTests
{
    public abstract class WhenTestingGenerateRefreshToken : WhenTestingJwtService
    {
        protected JwtRefreshToken Result;

        protected InitiativeUser User;
        protected string Token;
        protected DateTime Expiration;

        [When]
        public void GenerateRefreshTokenIsCalled()
        {
            Result = JwtService.GenerateRefreshToken(User, Expiration);
        }
    }
}
