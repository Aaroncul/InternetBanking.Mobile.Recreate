using InternetBanking.ViewModels.Base;
using System.Threading.Tasks;

namespace InternetBanking.ViewModels
{
    public class MainViewModel : SkinnedViewModel
    {
        private MenuViewModel _menuViewModel;

        public MenuViewModel MenuViewModel
        {
            get => _menuViewModel;
            set
            {
                _menuViewModel = value;
                RaisePropertyChanged(() => MenuViewModel);
            }
        }

        public MainViewModel(MenuViewModel menuViewModel)
        {
            _menuViewModel = menuViewModel;
        }

        public override Task InitializeAsync(object navigationData)
        {
            return Task.WhenAll
            (
                _menuViewModel.InitializeAsync(navigationData),
                NavigationService.NavigateToAsync<AccountsViewModel>()
            );
        }
    }
}