using System;

namespace InternetBanking.Validation
{
    public sealed class IsGreaterThanZero<T> : BaseValidationRule<T>
    {
        public override bool Check(T value)
        {
            try
            {
                var a = Convert.ToDecimal(value);
                if (a <= 0)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}