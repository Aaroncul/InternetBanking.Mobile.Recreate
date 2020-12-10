using System.Text.RegularExpressions;

namespace InternetBanking.Validation
{
    public sealed class IsValidMobileNumberRule<T> : BaseValidationRule<T>
    {
        private readonly int _length;

        public IsValidMobileNumberRule(int length)
        {
            _length = length;
        }

        public override bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            var str = (value as string).Replace(" ", "");

            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            // matches strings that only contain digits
            return str.Length == _length && new Regex(@"^(07)([0-9]{9})$").IsMatch(str);
        }
    }
}