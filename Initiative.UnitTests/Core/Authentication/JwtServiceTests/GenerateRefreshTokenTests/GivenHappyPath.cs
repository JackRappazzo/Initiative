using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Core.Authentication.JwtServiceTests.GenerateRefreshTokenTests
{
    public class GivenHappyPath : WhenTestingGenerateRefreshToken
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIsSet)
            .And(ExpirationIsSet)
            .When(GenerateRefreshTokenIsCalled)
            .Then(ShouldReturnValidToken)
            .And(ShouldStoreToken);

        [Given]
        public void UserIsSet()
        {
            User = new ApplicationIdentity()
            {
                Email = "sample@email.com",
                UserName = "username",
            };
        }

        [Given]
        public void ExpirationIsSet()
        {
            Expiration = DateTime.Now + TimeSpan.FromDays(30);
        }

        [Then]
        public void ShouldReturnValidToken()
        {
            Assert.That(Result.User.Email, Is.EqualTo(User.Email));
            Assert.That(Result.RefreshToken, Is.Not.Null);
            Assert.That(Result.RefreshToken.Count(), Is.GreaterThan(0));
            Assert.That(Result.Expiration, Is.EqualTo(Expiration));
        }

        [Then]
        public void ShouldStoreToken()
        {
            JwtRefreshTokenRepository
                .Received(1)
                .UpsertRefreshToken(Result.User.Id.ToString(), Result.RefreshToken, Result.Expiration, Arg.Any<CancellationToken>());
        }
    }
}
