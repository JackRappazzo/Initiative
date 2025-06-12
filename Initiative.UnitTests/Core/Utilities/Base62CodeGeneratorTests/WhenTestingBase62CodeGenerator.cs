using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Initiative.Api.Core.Utilities;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.Core.Composable;
using LeapingGorilla.Testing.NUnit.Attributes;
using LeapingGorilla.Testing.NUnit.Composable;

namespace Initiative.UnitTests.Core.Utilities.Base62CodeGeneratorTests
{
    public class WhenTestingBase62CodeGenerator : ComposableTestingTheBehaviourOf
    {
        [ItemUnderTest] protected Base62CodeGenerator CodeGenerator;

        protected int CodeLength;
        protected string Result;

        protected override ComposedTest ComposeTest() => TestComposer
            .Given(CodeLengthIsTen)
            .When(GenerateCodeIsCalled)
            .Then(ResultIsNotNullOrEmpty)
            .And(ResultLengthIsEqualToCodeLength)
            .And(ResultContainsOnlyBase62Characters);

        [Given]
        public void CodeLengthIsTen()
        {
            CodeLength = 10;
        }

        [When]
        public void GenerateCodeIsCalled()
        {
            Result = CodeGenerator.GenerateCode(CodeLength);
        }

        [Then]
        public void ResultIsNotNullOrEmpty()
        {
            Assert.That(string.IsNullOrEmpty(Result), Is.False);
        }

        [Then]
        public void ResultLengthIsEqualToCodeLength()
        {
            Assert.That(Result.Length, Is.EqualTo(CodeLength));
        }

        [Then]
        public void ResultContainsOnlyBase62Characters()
        {
            const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            Assert.That(Result.All(c => Base62Chars.Contains(c)), Is.True, "Result contains invalid characters.");
        }

    }
}
