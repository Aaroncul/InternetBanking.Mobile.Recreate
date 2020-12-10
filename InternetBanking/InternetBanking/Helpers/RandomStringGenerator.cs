using System;
using System.Linq;
using System.Threading;

namespace InternetBanking.Helpers
{
    public class RandomStringGenerator
    {
        private const string UppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";
        private const string Numbers = "0123456789";
        private const string ExtendedCharacters = "~!@#$%^&*()_+-=[]{};:,.<>?";

        private static readonly ThreadLocal<Random> Random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        private static int _seed = Environment.TickCount;

        public string Create(int length)
        {
            return CreateImpl(length, UppercaseCharacters, LowercaseCharacters);
        }

        public string Create(int length, bool lettersOnly)
        {
            return lettersOnly ?
                CreateImpl(length, UppercaseCharacters, LowercaseCharacters) :
                CreateImpl(length, UppercaseCharacters, LowercaseCharacters, Numbers, ExtendedCharacters);
        }

        public string CreateAlphanumeric(int length)
        {
            return CreateImpl(length, UppercaseCharacters, LowercaseCharacters, Numbers);
        }

        public string CreateNumber(int length)
        {
            return CreateImpl(length, Numbers);
        }

        private static string CreateImpl(int length, params string[] arrays)
        {
            return new string(Enumerable.Repeat(string.Concat(arrays), length)
                .Select(x => x[Random.Value.Next(x.Length)])
                .ToArray());
        }
    }
}
