using InternetBanking.Services.Dialogs;
using InternetBanking.Services.Navigation;
using InternetBanking.ViewModels.Base;
using System.Threading.Tasks;

namespace InternetBanking.ViewModels
{
    public abstract class BaseViewModel : ExtendedBindableObject
    {
        protected readonly INavigationService NavigationService;
        protected readonly IDialogService DialogService;

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

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

        protected BaseViewModel()
        {
            NavigationService = ViewModelLocator
                .Instance
                .Resolve<INavigationService>();

            DialogService = ViewModelLocator
                .Instance
                .Resolve<IDialogService>();
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }
    }
}