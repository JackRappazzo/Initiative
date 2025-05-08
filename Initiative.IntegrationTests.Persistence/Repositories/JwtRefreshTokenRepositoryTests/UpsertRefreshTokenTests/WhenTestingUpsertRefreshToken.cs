using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;

namespace Initiative.IntegrationTests.Persistence.Repositories.JwtRefreshTokenRepositoryTests.UpsertRefreshTokenTests
{
    public abstract class WhenTestingUpsertRefreshToken : WhenTestingJwtRefreshTokenRepository
    {
        public string UserId;
        public string RefreshToken;
        public DateTime Expiration;

        [When]
        public async Task UpsertIsCalled()
        {
            await JwtRefreshTokenRepository.UpsertRefreshToken(UserId, RefreshToken, Expiration, CancellationToken);
        }

    }
}
