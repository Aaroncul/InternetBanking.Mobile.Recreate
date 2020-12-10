using System;

namespace InternetBanking.Validation
{
    public sealed class IsValidSortCode<T> : BaseValidationRule<T>
    {
        public override bool Check(T value)
        {
            try
            {
                var s = value.ToString().Replace("-", "");
                Convert.ToInt64(s);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}