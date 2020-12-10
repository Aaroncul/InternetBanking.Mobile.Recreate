using System;

namespace InternetBanking.Validation
{
    public sealed class IsNotMoreThan20<T> : BaseValidationRule<T>
    {
        public override bool Check(T value)
        {
            try
            {
                var a = Convert.ToString(value);
                if (a.Length >= 20)
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