using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Persistence.Models.Authentication;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.JwtRefreshTokenRepositoryTests.FetchTokenTests
{
    public class GivenTokenExists : WhenTestingFetchToken
    {
        protected JwtRefreshTokenModel ExpectedToken;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(TokenExists)
            .When(FetchTokenIsCalled)
            .Then(ShouldReturnToken);

        [Given]
        public async Task TokenExists()
        {

            RefreshToken = DateTime.UtcNow.ToFileTimeUtc().ToString();

            ExpectedToken = new JwtRefreshTokenModel()
            {
                UserId = ObjectId.GenerateNewId(),
                Expiration = DateTime.UtcNow.AddDays(5),
                RefreshToken = RefreshToken
            };

            await JwtRefreshTokenRepository.UpsertRefreshToken(ExpectedToken.UserId.ToString(), ExpectedToken.RefreshToken, ExpectedToken.Expiration, CancellationToken);
        }

        [Then]
        public void ShouldReturnToken()
        {
            Assert.That(Result.UserId, Is.EqualTo(ExpectedToken.UserId));
            Assert.That(Result.RefreshToken, Is.EqualTo(ExpectedToken.RefreshToken));
        }
    }
}
