using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Authentication;
using Initiative.Api.Core.Identity;
using Initiative.Api.Services;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Api.Services.LoginServiceTests.LoginTests
{
    public class GivenHappyPath : WhenTestingLogin
    {
        private const string ExpectedRefreshToken = "refresh-token";
        InitiativeUser TestUser;
        string ExpectedJwt = "jwt";

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EmailAndPasswordAreSet)
            .And(UserManagerCanFindUser)
            .And(PasswordMatches)
            .And(JwtServiceReturnsJwt)
            .And(JwtServiceGeneratesRefreshToken)
            .When(LoginIsCalled)
            .Then(ShouldReturnSuccess)
            .And(ShouldNotHaveErrorMessage)
            .And(ShouldReturnExpectedJwt)
            .And(ShouldReturnExpectedRefreshToken);


        [Given]
        public void EmailAndPasswordAreSet()
        {
            Email = "email@address.com";
            Password = "password";
        }

        [Given]
        public void UserManagerCanFindUser()
        {
            TestUser = new InitiativeUser()
            {
                Email = Email,
                DisplayName = "TestUser"
            };

            UserManager.FindByEmailAsync(Email)
                .Returns(TestUser);
        }

        [Given]
        public void PasswordMatches()
        {
            UserManager.CheckPasswordAsync(Arg.Is<InitiativeUser>(u => u.Email == Email), Password)
                .Returns(true);
        }

        [Given]
        public void JwtServiceReturnsJwt()
        {
            JwtService.GenerateToken(Arg.Is<InitiativeUser>(u => u.Email == Email))
                .Returns(ExpectedJwt);
        }

        [Given]
        public void JwtServiceGeneratesRefreshToken()
        {
            JwtService.GenerateAndStoreRefreshToken(Arg.Is<InitiativeUser>(u => u.Email == Email), Arg.Any<DateTime>(), CancellationToken)
                .Returns(argInfo => new JwtRefreshToken()
                {
                    Expiration = argInfo.ArgAt<DateTime>(1),
                    RefreshToken = ExpectedRefreshToken,
                    User = argInfo.ArgAt<InitiativeUser>(0)
                });
        }

        [Then]
        public void ShouldReturnSuccess()
        {
            Assert.That(Result.Success, Is.True);
        }

        [Then]
        public void ShouldNotHaveErrorMessage()
        {
            Assert.That(Result.ErrorType, Is.EqualTo(LoginErrorType.None));
        }

        [Then]
        public void ShouldReturnExpectedJwt()
        {
            Assert.That(Result.Jwt, Is.EqualTo(ExpectedJwt));
        }

        [Then]
        public void ShouldReturnExpectedRefreshToken()
        {
            Assert.That(Result.RefreshToken, Is.EqualTo("refresh-token"));
        }
        
    }
}
