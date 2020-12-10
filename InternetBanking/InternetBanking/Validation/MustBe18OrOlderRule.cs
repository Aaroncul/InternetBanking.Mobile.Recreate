using System;

namespace InternetBanking.Validation
{
    public sealed class MustBe18OrOlderRule<T> : BaseValidationRule<T>
    {
        public override bool Check(T value)
        {
            return value != null && DateTime.Now.AddYears(-18) >= ((DateTime)(object)value);
        }
    }
}