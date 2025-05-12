using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Authentication;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Core.Authentication.JwtRefreshServiceTests.RefreshTokenTests
{
    public class GivenTokenIsExpired : WhenTestingRefreshToken
    {

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserExists)
            .And(RefreshTokenIsExpired)
            .When(RefreshIsCalled)
            .Then(ShouldNotReturnSuccess)
            .And(ShouldReturnNullToken);


        [Given]
        public void RefreshTokenIsExpired()
        {
            RefreshToken = "expired-token";

            JwtRefreshTokenRepository.FetchToken(RefreshToken, CancellationToken)
                .Returns(new JwtRefreshTokenModel()
                {
                    Expiration = DateTime.UtcNow - TimeSpan.FromDays(5),
                    RefreshToken = RefreshToken
                });
        }

      
    }
}
