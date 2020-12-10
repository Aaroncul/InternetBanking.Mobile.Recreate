using System;

namespace InternetBanking.Validation
{
    public sealed class IsValidBankAccountNumber<T> : BaseValidationRule<T>
    {
        public override bool Check(T value)
        {
            try
            {
                return Math.Floor(Math.Log10(Convert.ToInt64(value)) + 1) == 8;
            }
            catch
            {
                return false;
            }
        }
    }
}