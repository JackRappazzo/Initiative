using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Authentication;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.JwtRefreshTokenRepositoryTests.FetchTokenTests
{
    public abstract class WhenTestingFetchToken : WhenTestingJwtRefreshTokenRepository
    {

        protected JwtRefreshTokenModel Result;

        protected string RefreshToken;

        [When]
        public async Task FetchTokenIsCalled()
        {
            Result = await JwtRefreshTokenRepository.FetchToken(RefreshToken, CancellationToken);
        }
    }
}
