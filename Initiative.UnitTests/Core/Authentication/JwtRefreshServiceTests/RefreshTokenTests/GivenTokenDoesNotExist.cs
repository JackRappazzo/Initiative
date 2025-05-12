using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Initiative.UnitTests.Core.Authentication.JwtRefreshServiceTests.RefreshTokenTests
{
    public class GivenTokenDoesNotExist : WhenTestingRefreshToken
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(RefreshTokenDoesNotExist)
            .When(RefreshIsCalled)
            .Then(ShouldNotReturnSuccess)
            .And(ShouldReturnNullToken);

        [Given]
        public void RefreshTokenDoesNotExist()
        {
            RefreshToken = "invalid-token";
            JwtRefreshTokenRepository.FetchToken(RefreshToken, CancellationToken)
                .ReturnsNull();
        }

        
    }
}
