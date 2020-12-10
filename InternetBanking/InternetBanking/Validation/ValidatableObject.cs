using InternetBanking.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;

namespace InternetBanking.Validation
{
    public class ValidatableObject<T> : ExtendedBindableObject, IValidity
    {
        public List<IValidationRule<T>> Validations { get; }

        private bool _isValid;

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                _isValid = value;
                RaisePropertyChanged(() => IsValid);
            }
        }

        private List<string> _errors;

        public List<string> Errors
        {
            get { return _errors; }
            set
            {
                _errors = value;
                RaisePropertyChanged(() => Errors);
            }
        }

        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        public ValidatableObject()
        {
            _isValid = true;
            _errors = new List<string>();

            Validations = new List<IValidationRule<T>>();
        }

        public bool Validate()
        {
            Errors.Clear();

            var errors = Validations.Where(x => !x.Check(Value))
                                    .Select(x => x.ValidationMessage);

            Errors = errors.ToList();
            IsValid = !Errors.Any();

            return IsValid;
        }
    }
}