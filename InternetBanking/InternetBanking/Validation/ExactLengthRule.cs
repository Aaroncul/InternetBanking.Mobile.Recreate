namespace InternetBanking.Validation
{
    public sealed class ExactLengthRule<T> : BaseValidationRule<T>
    {
        private readonly int _length;

        public ExactLengthRule(int length)
        {
            _length = length;
        }

        public override bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            var str = value as string;

            return str.Length == _length;
        }
    }
}