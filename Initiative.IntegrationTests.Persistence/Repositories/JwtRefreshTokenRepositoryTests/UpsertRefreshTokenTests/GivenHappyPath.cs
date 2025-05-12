using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.JwtRefreshTokenRepositoryTests.UpsertRefreshTokenTests
{
    public class GivenHappyPath : WhenTestingUpsertRefreshToken
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(UserIdIsSet)
            .And(TokenIsSet)
            .And(ExpirationIsSet)
            .When(UpsertIsCalled)
            .Then(ShouldUpsert);

        [Given]
        public void UserIdIsSet()
        {
            UserId = ObjectId.GenerateNewId().ToString();
        }

        [Given]
        public void TokenIsSet()
        {
            RefreshToken = "test-token" + DateTime.UtcNow.ToFileTimeUtc().ToString();
        }

        [Given]
        public void ExpirationIsSet()
        {
            Expiration = DateTime.Now + TimeSpan.FromDays(30);
        }



        [Then]
        public async Task ShouldUpsert()
        {
            var token = await JwtRefreshTokenRepository.FetchToken(RefreshToken, CancellationToken);

            Assert.That(token, Is.Not.Null);
            Assert.That(token.UserId, Is.EqualTo(new ObjectId(UserId)));
            Assert.That(token.RefreshToken, Is.EqualTo(RefreshToken));

        }
    }
}
