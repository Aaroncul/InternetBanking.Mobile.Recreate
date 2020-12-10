using InternetBanking.Services.Authentication;
using InternetBanking.ViewModels;
using InternetBanking.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InternetBanking.Services.Navigation
{
    public class iOSNavigationService : NavigationService
    {
        public iOSNavigationService(IAuthenticationService authenticationService)
            : base(authenticationService)
        {
            if (Mappings.ContainsKey(typeof(MainViewModel)))
            {
                Mappings[typeof(MainViewModel)] = typeof(iOSMainView);
            }
            else
            {
                Mappings.Add(typeof(MainViewModel), typeof(iOSMainView));
            }
        }

        private void InitalizeMainPage(iOSMainView mainPage)
        {
            Application.Current.MainPage = mainPage;

            var accountsPage = CreateAndBindPage(typeof(AccountsViewModel), null);
            var settingsPage = CreateAndBindPage(typeof(SettingsViewModel), null);
            var morePage = CreateAndBindPage(typeof(MoreViewModel), null);

            if (Device.RuntimePlatform == Device.Android)
            {
                mainPage.AddPage(accountsPage, "");
                mainPage.AddPage(settingsPage, "");
                mainPage.AddPage(morePage, "");
            }
            else
            {
                mainPage.AddPage(accountsPage, "Accounts");
                mainPage.AddPage(settingsPage, "Profile");
                mainPage.AddPage(morePage, "More");
            }
        }

        private Task InitializePageViewModelAsync(Page page, object parameter)
        {
            return (page.BindingContext as BaseViewModel).InitializeAsync(parameter);
        }

        private Task InitializeTabPageCurrentPageViewModelAsync(object parameter)
        {
            var mainPage = Application.Current.MainPage as iOSMainView;
            return ((mainPage.CurrentPage as CustomNavigationView).CurrentPage.BindingContext as BaseViewModel).InitializeAsync(parameter);
        }

        protected override async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreateAndBindPage(viewModelType, parameter);

            if (page is iOSMainView)
            {
                InitalizeMainPage(page as iOSMainView);
                await InitializeTabPageCurrentPageViewModelAsync(parameter);
            }
            else if (page is LoginView)
            {
                Application.Current.MainPage = new CustomNavigationView(page);
                await InitializePageViewModelAsync(page, parameter);
            }
            else if (Application.Current.MainPage is iOSMainView)
            {
                var mainPage = Application.Current.MainPage as iOSMainView;
                bool tabPageFound = mainPage.TrySetCurrentPage(page);

                if (!tabPageFound)
                {
                    await mainPage.CurrentPage.Navigation.PushAsync(page);
                    await InitializePageViewModelAsync(page, parameter);
                }
            }
            else
            {
                if (Application.Current.MainPage is CustomNavigationView navigationPage)
                {
                    await navigationPage.PushAsync(page);
                }
                else
                {
                    Application.Current.MainPage = new CustomNavigationView(page);
                }

                await InitializePageViewModelAsync(page, parameter);
            }
        }

        private async void OnTabPageAppearing(object sender, EventArgs e)
        {
            await InitializeTabPageCurrentPageViewModelAsync(null);
        }
    }
}

