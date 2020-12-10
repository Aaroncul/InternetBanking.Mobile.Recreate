using InternetBanking.ViewModels.Base;
using System;

namespace InternetBanking.ViewModels.Components
{
    public class MenuItem : ExtendedBindableObject
    {
        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }
        public AwesomeIcon Icon { get; set; }


        private Type _viewModelType;

        public Type ViewModelType
        {
            get => _viewModelType;
            set
            {
                _viewModelType = value;
                RaisePropertyChanged(() => ViewModelType);
            }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                RaisePropertyChanged(() => IsEnabled);
            }
        }
    }
}