using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using Initiative.Api.Core.Services.Authentication;
using Initiative.Persistence.Models.Authentication;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Core.Authentication.JwtRefreshServiceTests.RefreshTokenTests
{
    public class GivenTokenIsValid : WhenTestingRefreshToken
    {

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserExists)
            .And(RefreshTokenNotExpired)
            .And(JwtSettingsAreSet)
            .And(JwtServiceCanGenerateToken)
            .When(RefreshIsCalled)
            .Then(ShouldReturnSuccess)
            .And(ShouldReturnToken)
            .And(ShouldExtendRefreshTokenExpiration);


        [Given]
        public void RefreshTokenNotExpired()
        {
            RefreshToken = "refresh-token";

            JwtRefreshTokenRepository.FetchToken(RefreshToken, CancellationToken)
                .Returns(new JwtRefreshTokenModel()
                {
                    Expiration = DateTime.UtcNow.AddDays(5),
                    RefreshToken = RefreshToken,
                    Id = ObjectId.GenerateNewId(),
                    UserId = UserId
                });
        }

        [Given]
        public void JwtSettingsAreSet()
        {
            JwtSettings = new JwtSettings()
            {
                Secret = "secret",
                Audience = "audience",
                ExpiresInMinutes = 10,
                Issuer = "unittest",
                RefreshTokenExpiresInDays = 60
            };

            JwtSettingsContainer.Value.Returns(JwtSettings);
        }

        [Given]
        public void JwtServiceCanGenerateToken()
        {
            ExpectedToken = "jwt-string";
            JwtService.GenerateToken(Arg.Is<ApplicationIdentity>(i => i.Id == UserId))
                .Returns(ExpectedToken);
        }

        [Then]
        public void ShouldReturnSuccess()
        {
            Assert.That(ResultSuccess, Is.True);
        }

        [Then]
        public void ShouldReturnToken()
        {
            Assert.That(ResultToken, Is.Not.Null);
            Assert.That(ResultToken, Is.EqualTo(ExpectedToken));
        }

        [Then]
        public void ShouldExtendRefreshTokenExpiration()
        {
            JwtRefreshTokenRepository.Received()
                .UpsertRefreshToken(
                    Arg.Is<string>(s => s == UserId.ToString()),
                    Arg.Is<string>(s => s == RefreshToken),
                    Arg.Is<DateTime>(d => d > DateTime.UtcNow.AddDays(59) && d < DateTime.UtcNow.AddDays(61)),
                    Arg.Any<CancellationToken>());
        }
    }
}
