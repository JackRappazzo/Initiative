using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Identity;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using NSubstitute;

namespace Initiative.UnitTests.Api.Services.LoginServiceTests.LoginTests
{
    public class GivenHappyPath : WhenTestingLogin
    {

        InitiativeUser TestUser;
        string ExpectedJwt = "jwt";

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(EmailAndPasswordAreSet)
            .And(UserManagerCanFindUser)
            .And(PasswordMatches)
            .And(JwtServiceReturnsJwt)
            .When(LoginIsCalled)
            .Then(ShouldReturnSuccess)
            .And(ShouldNotHaveErrorMessage)
            .And(ShouldReturnExpectedJwt);


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

        [Then]
        public void ShouldReturnSuccess()
        {
            Assert.That(IsSuccess, Is.True);
        }

        [Then]
        public void ShouldNotHaveErrorMessage()
        {
            Assert.That(string.IsNullOrEmpty(ErrorMessage), Is.True);
        }

        [Then]
        public void ShouldReturnExpectedJwt()
        {
            Assert.That(Jwt, Is.EqualTo(ExpectedJwt));
        }
        
    }
}
