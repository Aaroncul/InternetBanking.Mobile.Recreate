using System;

namespace InternetBanking.Validation
{
    public sealed class IsLessThanAvailableBalance<T> : BaseValidationRule<T>
    {
        private decimal AvailableAmount;

        public IsLessThanAvailableBalance(decimal availableAmount)
        {
            AvailableAmount = availableAmount;
        }

        public override bool Check(T value)
        {
            try
            {
                var amount = Convert.ToDecimal(value);
                return amount <= AvailableAmount;
            }
            catch
            {
                return false;
            }
        }
    }
}