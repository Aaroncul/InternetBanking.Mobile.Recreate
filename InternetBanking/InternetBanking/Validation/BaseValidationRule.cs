namespace InternetBanking.Validation
{
    public abstract class BaseValidationRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public abstract bool Check(T value);
    }
}