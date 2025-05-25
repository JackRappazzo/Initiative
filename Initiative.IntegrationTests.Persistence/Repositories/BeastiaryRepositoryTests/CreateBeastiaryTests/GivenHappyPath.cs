using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using MongoDB.Bson;

namespace Initiative.IntegrationTests.Persistence.Repositories.BeastiaryRepositoryTests.CreateSystemBeastiaryTests
{
    public class GivenHappyPath : WhenTestingCreateUserBeastiary
    {
        protected override ComposedTest ComposeTest() => TestComposer
            .Given(BeastiaryNameIsSet)
            .When(CreateIsCalled)
            .Then(ShouldReturnNewId)
            .And(ShouldStoreBeastiary);

        [Given]
        public void BeastiaryNameIsSet()
        {
            Name = "test-beastiary";
        }

        [Then]
        public void ShouldReturnNewId()
        {
            Assert.That(Result, Is.Not.Null);
            Assert.That(Result, Is.Not.Empty);
        }

        [Then]
        public async Task ShouldStoreBeastiary()
        {
            var beastiary = await BeastiaryRepository.GetSystemBeastiary(Result, CancellationToken);

            Assert.That(beastiary.Id, Is.EqualTo(Result));
            Assert.That(beastiary.Name, Is.EqualTo(Name));
        }

    }
}
