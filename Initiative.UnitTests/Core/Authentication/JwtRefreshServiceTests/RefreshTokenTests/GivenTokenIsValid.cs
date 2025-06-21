using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
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
            .And(JwtServiceCanGenerateToken)
            .When(RefreshIsCalled)
            .Then(ShouldReturnSuccess)
            .And(ShouldReturnToken);


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
    }
}
