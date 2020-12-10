using InternetBanking.Dto;

namespace InternetBanking.Validation
{
    public sealed class IsNotNullOrEmptyRule<T> : BaseValidationRule<T>
    {
        public override bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            if (value.GetType() == typeof(AccountDto)
                || value.GetType() == typeof(BankAccountDto))
            {
                return true;
            }

            var str = value as string;

            return !string.IsNullOrWhiteSpace(str);
        }
    }
}