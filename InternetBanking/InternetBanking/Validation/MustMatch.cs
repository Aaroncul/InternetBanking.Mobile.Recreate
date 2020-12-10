using System;

namespace InternetBanking.Validation
{
    public sealed class MustMatch<T> : BaseValidationRule<T>
    {
        private ValidatableObject<string> MatchValue { get; set; }

        public MustMatch(ValidatableObject<string> matchValue)
        {
            MatchValue = matchValue;
        }

        public override bool Check(T value)
        {
            try
            {
                return value.ToString() == MatchValue.Value;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}