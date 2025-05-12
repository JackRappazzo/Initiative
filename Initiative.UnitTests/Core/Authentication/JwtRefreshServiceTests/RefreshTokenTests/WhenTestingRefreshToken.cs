using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Authentication;
using Initiative.Api.Core.Identity;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;
using NSubstitute;

namespace Initiative.UnitTests.Core.Authentication.JwtRefreshServiceTests.RefreshTokenTests
{
    public abstract class WhenTestingRefreshToken : WhenTestingJwtRefreshService
    {
        protected bool ResultSuccess;
        protected string? ResultToken;

        protected string RefreshToken;

        protected ObjectId UserId;

        protected string ExpectedToken;

        [When]
        public async Task RefreshIsCalled()
        {
            (ResultSuccess, ResultToken) = await JwtRefreshService.RefreshJwt(RefreshToken, CancellationToken);
        }

        [Given]
        public void UserExists()
        {
            UserId = ObjectId.GenerateNewId();
            UserManager.FindByIdAsync(UserId.ToString())
                .Returns(new InitiativeUser()
                {
                    Id = UserId,
                    Email = "test@email.com",
                    DisplayName = "TestUser"
                });
        }

        [Then]
        public void ShouldNotReturnSuccess()
        {
            Assert.That(ResultSuccess, Is.False);
        }

        [Then]
        public void ShouldReturnNullToken()
        {
            Assert.That(ResultToken, Is.Null);
        }
    }
}
