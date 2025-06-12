using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiative.Api.Core.Utilities
{
    public class Base62CodeGenerator : IBase62CodeGenerator
    {
        private Random random = new Random();

        private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public string GenerateCode(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Length must be a positive integer.", nameof(length));
            }

            var data = new byte[length];
            random.NextBytes(data);

            var sb = new StringBuilder(length);
            foreach (var b in data)
            {
                sb.Append(Base62Chars[b % Base62Chars.Length]);
            }
            return sb.ToString();
        }
    }
}
