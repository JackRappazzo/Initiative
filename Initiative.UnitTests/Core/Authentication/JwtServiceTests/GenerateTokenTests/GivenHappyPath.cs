using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Authentication;
using Initiative.Api.Core.Identity;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using SharpCompress.Common;

namespace Initiative.UnitTests.Core.Authentication.JwtServiceTests.GenerateTokenTests
{
    public class GivenHappyPath : WhenTestingCreate
    {

        SigningCredentials Credentials;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIsSet)
            .And(SettingsAreSet)
            .And(CredentialFactoryCreatesCredentials)
            .When(CreateIsCalled)
            .Then(ShouldNotReturnNull);


        [Given]
        public void UserIsSet()
        {
            User = new InitiativeUser()
            {
                UserName = "TestUser",
                Email = "TestEmail@test.com"
            };
        }

        [Given]
        public void SettingsAreSet()
        {
            JwtSettings = new JwtSettings()
            {
                Secret = "secret",
                Audience = "audience",
                ExpiresInMinutes = 10,
                Issuer = "unittest"
            };

            JwtSettingsContainer.Value.Returns(JwtSettings);
        }

        [Given]
        public void CredentialFactoryCreatesCredentials()
        {

            var randomKey = new SymmetricSecurityKey(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
            Credentials = new SigningCredentials(randomKey, SecurityAlgorithms.HmacSha256);

            CredentialsFactory.Create(JwtSettingsContainer.Value.Secret).Returns(Credentials);
        }

        [Then]
        public void ShouldNotReturnNull()
        {
            Assert.That(Result, Is.Not.Null);
        }
    }
}
